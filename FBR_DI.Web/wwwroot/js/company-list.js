$(function () {
    $('#companiesTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Admin/Company/GetAll',
            type: 'GET',
            data: function (d) {
                return {
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search.value,
                    sortColumn: d.columns[d.order[0].column].name,
                    sortDir: d.order[0].dir
                };
            }
        },
        columns: [
            { data: 'companyName', name: 'companyname' },
            { data: 'ntn', name: 'ntn' },
            { data: 'province', name: 'province' },
            {
                data: 'isIntegrated', name: 'isIntegrated', orderable: false,
                render: function (d) {
                    return d
                        ? '<span class="badge-success">Integrated</span>'
                        : '<span class="badge-danger">Not Integrated</span>';
                }
            },
            {
                data: 'environment', name: 'environment', orderable: false,
                render: function (d) {
                    return d === 2
                        ? '<span class="badge-success">Production</span>'
                        : '<span class="badge-warning">Sandbox</span>';
                }
            },
            {
                data: 'id', orderable: false,
                render: function (id) {
                    return `
                        <div class="flex gap-2">
                            <a href="/Admin/Company/Edit/${id}" class="text-blue-600 hover:underline text-xs font-medium">Edit</a>
                            <a href="/Admin/Company/FbrSettings/${id}" class="text-yellow-600 hover:underline text-xs font-medium">FBR Settings</a>
                            <button onclick="deleteCompany(${id})" class="text-red-600 hover:underline text-xs font-medium">Delete</button>
                        </div>`;
                }
            }
        ],
        pageLength: 10,
        language: { processing: '<div class="text-gray-400 text-sm">Loading...</div>' }
    });
});

function deleteCompany(id) {
    Swal.fire({
        title: 'Delete Company?',
        text: 'This will soft-delete the company and all its data.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc2626',
        confirmButtonText: 'Yes, Delete'
    }).then(result => {
        if (!result.isConfirmed) return;
        $.post(`/Admin/Company/Delete/${id}`, {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').first().val()
        }, function (data) {
            if (data.success) {
                toastr.success(data.message || 'Deleted successfully.');
                $('#companiesTable').DataTable().ajax.reload();
            } else {
                toastr.error(data.message || 'Failed to delete.');
            }
        });
    });
}
