let invoicesTable;
const selectedIds = new Set();

$(function () {
    invoicesTable = $('#invoicesTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Admin/Invoice/GetAll',
            type: 'GET',
            data: function (d) {
                return {
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search.value,
                    status: $('#filterStatus').val(),
                    fromDate: $('#filterFrom').val(),
                    toDate: $('#filterTo').val()
                };
            }
        },
        columns: [
            {
                data: 'id', orderable: false, className: 'text-center',
                render: function (id) {
                    return `<input type="checkbox" class="row-checkbox h-4 w-4" data-id="${id}" />`;
                }
            },
            {
                data: 'fbrInvoiceNumber', orderable: false,
                render: function (d, t, row) {
                    return d
                        ? `<span class="font-mono text-xs text-gray-700">${d}</span>`
                        : `<span class="text-gray-400 text-xs">Not submitted</span>`;
                }
            },
            {
                data: 'invoiceDate',
                render: function (d) { return d ? new Date(d).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }) : ''; }
            },
            { data: 'buyerBusinessName' },
            {
                data: 'grandTotal',
                render: function (d) { return 'PKR ' + parseFloat(d).toLocaleString('en-US', { minimumFractionDigits: 2 }); }
            },
            {
                data: 'status', orderable: false,
                render: function (d) {
                    const map = { 0: 'badge-gray', 1: 'badge-warning', 2: 'badge-warning', 3: 'badge-success', 4: 'badge-danger', 5: 'badge-danger' };
                    const labels = { 0: 'Draft', 1: 'Pending', 2: 'Submitted', 3: 'Valid', 4: 'Invalid', 5: 'Failed' };
                    return `<span class="${map[d] || 'badge-gray'}">${labels[d] || d}</span>`;
                }
            },
            {
                data: null, orderable: false,
                render: function (d, t, row) {
                    let html = `<div class="flex gap-2">
                        <a href="/Admin/Invoice/Detail/${row.id}" class="text-blue-600 text-xs font-medium hover:underline">View</a>`;
                    if (row.status === 0) {
                        html += `<a href="/Admin/Invoice/Edit/${row.id}" class="text-gray-600 text-xs font-medium hover:underline">Edit</a>`;
                        html += `<button onclick="submitSingle(${row.id})" class="text-green-600 text-xs font-medium hover:underline">Submit</button>`;
                        html += `<button onclick="deleteInvoice(${row.id})" class="text-red-600 text-xs font-medium hover:underline">Delete</button>`;
                    }
                    if (row.status === 4) {
                        html += `<button onclick="submitSingle(${row.id})" class="text-green-600 text-xs font-medium hover:underline">Resubmit</button>`;
                    }
                    html += `<a href="/Admin/Invoice/PrintInvoice/${row.id}" target="_blank" class="text-gray-500 text-xs font-medium hover:underline">Print</a>`;
                    html += '</div>';
                    return html;
                }
            }
        ],
        pageLength: 10,
        drawCallback: function () {
            // Re-bind row checkboxes after redraw
            $('.row-checkbox').off('change').on('change', function () {
                const id = parseInt($(this).data('id'));
                if (this.checked) selectedIds.add(id); else selectedIds.delete(id);
                updateBulkButton();
            });
        }
    });

    // Select All
    $('#selectAll').on('change', function () {
        $('.row-checkbox').prop('checked', this.checked).trigger('change');
    });
});

function reloadTable() {
    invoicesTable.ajax.reload();
}

function updateBulkButton() {
    const count = selectedIds.size;
    if (count > 0) {
        $('#bulkSubmitBtn').removeClass('hidden');
        $('#selectedCount').text(count);
    } else {
        $('#bulkSubmitBtn').addClass('hidden');
    }
}

$('#bulkSubmitBtn').on('click', function () {
    const ids = Array.from(selectedIds);
    Swal.fire({
        title: `Submit ${ids.length} Invoice(s)?`,
        text: 'All selected invoices will be submitted to FBR.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Submit All'
    }).then(r => {
        if (!r.isConfirmed) return;
        $.ajax({
            url: '/Admin/Invoice/BulkSubmit',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(ids),
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').first().val() },
            success: function (d) {
                toastr.success(`${d.data.successCount} submitted, ${d.data.failureCount} failed.`);
                selectedIds.clear();
                updateBulkButton();
                invoicesTable.ajax.reload();
            }
        });
    });
});

function submitSingle(id) {
    Swal.fire({ title: 'Submit to FBR?', icon: 'question', showCancelButton: true, confirmButtonText: 'Submit' })
    .then(r => {
        if (!r.isConfirmed) return;
        $.post(`/Admin/Invoice/Submit/${id}`,
            { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').first().val() },
            function (d) {
                if (d.success) {
                    toastr.success('Submitted! FBR#: ' + (d.invoiceNumber || 'Processing'));
                    invoicesTable.ajax.reload();
                } else {
                    toastr.error(d.message || d.error || 'Submission failed.');
                }
            }
        );
    });
}

function deleteInvoice(id) {
    Swal.fire({ title: 'Delete Invoice?', icon: 'warning', showCancelButton: true, confirmButtonColor: '#dc2626', confirmButtonText: 'Delete' })
    .then(r => {
        if (!r.isConfirmed) return;
        $.post(`/Admin/Invoice/Delete/${id}`,
            { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').first().val() },
            function (d) {
                if (d.success) { toastr.success('Deleted.'); invoicesTable.ajax.reload(); }
                else toastr.error(d.message);
            }
        );
    });
}
