let itemIndex = 0;

function addItemRow() {
    const container = document.getElementById('itemsContainer');
    document.getElementById('noItemsMsg').classList.add('hidden');

    const idx = itemIndex++;
    const saleTypeOptions = (typeof SALE_TYPES !== 'undefined' ? SALE_TYPES : [])
        .map(s => `<option value="${s}">${s}</option>`).join('');

    const row = document.createElement('div');
    row.className = 'border border-gray-200 rounded-xl p-4 space-y-3 relative';
    row.id = `item-row-${idx}`;
    row.innerHTML = `
        <div class="flex items-center justify-between">
            <span class="text-sm font-semibold text-gray-700">Item #<span class="item-num">${idx + 1}</span></span>
            <button type="button" onclick="removeRow(${idx})" class="text-red-500 hover:text-red-700 text-xs">
                <i class="fa fa-trash mr-1"></i>Remove
            </button>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-4 gap-3">
            <div>
                <label class="form-label">HS Code *</label>
                <input name="Items[${idx}].HsCode" class="form-input" placeholder="e.g. 1001.1000" required />
            </div>
            <div class="md:col-span-2">
                <label class="form-label">Product Description *</label>
                <input name="Items[${idx}].ProductDescription" class="form-input" required />
            </div>
            <div>
                <label class="form-label">Sale Type *</label>
                <select name="Items[${idx}].SaleType" class="form-input" required>
                    <option value="">Select...</option>
                    ${saleTypeOptions}
                </select>
            </div>
            <div>
                <label class="form-label">Rate *</label>
                <input name="Items[${idx}].Rate" class="form-input" placeholder="e.g. 18%" required />
            </div>
            <div>
                <label class="form-label">UoM *</label>
                <input name="Items[${idx}].UoM" class="form-input" placeholder="e.g. KGS" required />
            </div>
            <div>
                <label class="form-label">Quantity *</label>
                <input name="Items[${idx}].Quantity" type="number" step="0.0001" min="0.0001"
                       class="form-input calc-trigger" oninput="calculateTotals()" required />
            </div>
            <div>
                <label class="form-label">Total Values</label>
                <input name="Items[${idx}].TotalValues" type="number" step="0.01" class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">Value Excl. ST</label>
                <input name="Items[${idx}].ValueSalesExcludingST" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">Fixed/Notified Price</label>
                <input name="Items[${idx}].FixedNotifiedValueOrRetailPrice" type="number" step="0.01" class="form-input" value="0" />
            </div>
            <div>
                <label class="form-label">Sales Tax</label>
                <input name="Items[${idx}].SalesTaxApplicable" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">ST Withheld</label>
                <input name="Items[${idx}].SalesTaxWithheldAtSource" type="number" step="0.01" class="form-input" value="0" />
            </div>
            <div>
                <label class="form-label">Further Tax</label>
                <input name="Items[${idx}].FurtherTax" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">Extra Tax</label>
                <input name="Items[${idx}].ExtraTax" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">FED Payable</label>
                <input name="Items[${idx}].FedPayable" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">Discount</label>
                <input name="Items[${idx}].Discount" type="number" step="0.01"
                       class="form-input calc-trigger" oninput="calculateTotals()" value="0" />
            </div>
            <div>
                <label class="form-label">SRO Schedule No</label>
                <input name="Items[${idx}].SroScheduleNo" class="form-input" />
            </div>
            <div>
                <label class="form-label">SRO Item Serial No</label>
                <input name="Items[${idx}].SroItemSerialNo" class="form-input" />
            </div>
        </div>`;

    container.appendChild(row);
    renumberItems();
}

function removeRow(idx) {
    const row = document.getElementById(`item-row-${idx}`);
    if (row) row.remove();
    renumberItems();
    calculateTotals();
    if (document.getElementById('itemsContainer').children.length === 0)
        document.getElementById('noItemsMsg').classList.remove('hidden');
}

function renumberItems() {
    const rows = document.querySelectorAll('#itemsContainer > div');
    rows.forEach((row, i) => row.querySelector('.item-num').textContent = i + 1);
}

function getNum(selector) {
    const val = parseFloat(document.querySelector(selector)?.value || '0');
    return isNaN(val) ? 0 : val;
}

function calculateTotals() {
    let totalValueExclST = 0, totalSalesTax = 0, totalFurtherTax = 0;
    let totalExtraTax = 0, totalFedPayable = 0, totalDiscount = 0;

    document.querySelectorAll('#itemsContainer > div').forEach((row, i) => {
        const getValue = (name) => {
            const input = row.querySelector(`input[name$="].${name}"]`) ||
                          row.querySelector(`input[name*="[${i}].${name}"]`) ||
                          row.querySelector(`input[name$=".${name}"]`);
            // find by partial name match
            const all = row.querySelectorAll('input[type="number"]');
            for (const inp of all) {
                if (inp.name.endsWith(`.${name}`)) return parseFloat(inp.value || '0') || 0;
            }
            return 0;
        };
        totalValueExclST += getValue('ValueSalesExcludingST');
        totalSalesTax    += getValue('SalesTaxApplicable');
        totalFurtherTax  += getValue('FurtherTax');
        totalExtraTax    += getValue('ExtraTax');
        totalFedPayable  += getValue('FedPayable');
        totalDiscount    += getValue('Discount');
    });

    const grandTotal = totalValueExclST + totalSalesTax + totalFurtherTax + totalExtraTax + totalFedPayable - totalDiscount;

    document.getElementById('totalValueExclST').textContent = totalValueExclST.toFixed(2);
    document.getElementById('totalSalesTax').textContent    = totalSalesTax.toFixed(2);
    document.getElementById('totalFurtherTax').textContent  = totalFurtherTax.toFixed(2);
    document.getElementById('totalExtraTax').textContent    = totalExtraTax.toFixed(2);
    document.getElementById('totalFedPayable').textContent  = totalFedPayable.toFixed(2);
    document.getElementById('totalDiscount').textContent    = totalDiscount.toFixed(2);
    document.getElementById('grandTotal').textContent       = grandTotal.toFixed(2);
}

function onInvoiceTypeChange(sel) {
    const isDebit = sel.value === 'DebitNote';
    document.getElementById('invoiceRefNoField').classList.toggle('hidden', !isDebit);
}

function onBuyerRegTypeChange(sel) {
    const isRegistered = sel.value === 'Registered';
    document.getElementById('buyerNtnField').classList.toggle('hidden', !isRegistered);
}

function onCompanyChange(sel) {
    // Auto-populate seller fields from company data if needed
    // For now just show scenario ID field logic
    const companyId = sel.value;
    if (companyId) {
        document.getElementById('scenarioIdField').classList.remove('hidden');
    }
}

function checkBuyerStatus() {
    const ntn = document.querySelector('input[name="BuyerNTNCNIC"]')?.value;
    const date = document.querySelector('input[name="InvoiceDate"]')?.value;
    const companyId = document.querySelector('select[name="CompanyId"]')?.value;
    const resultEl = document.getElementById('buyerStatusResult');

    if (!ntn || !companyId) {
        resultEl.className = 'text-xs mt-1 text-red-500';
        resultEl.textContent = 'Select a company and enter buyer NTN/CNIC first.';
        return;
    }

    resultEl.className = 'text-xs mt-1 text-gray-400';
    resultEl.textContent = 'Checking...';

    fetch(`/Admin/ReferenceData/CheckBuyerStatus?companyId=${companyId}&regno=${ntn}&date=${date || new Date().toISOString().split('T')[0]}`)
    .then(r => r.json())
    .then(d => {
        resultEl.className = d.success ? 'text-xs mt-1 text-green-600' : 'text-xs mt-1 text-red-500';
        resultEl.textContent = d.message || (d.success ? 'Status: Active' : 'Status check failed.');
    });
}

// Init — load companies dropdown + set today as default invoice date
$(function () {
    // Set default invoice date
    const dateInput = $('input[name="InvoiceDate"]');
    if (!dateInput.val()) {
        dateInput.val(new Date().toISOString().split('T')[0]);
    }

    // Load companies via AJAX
    const select = $('#companySelect');
    $.getJSON('/Admin/Company/GetAllForDropdown', function (data) {
        data.forEach(function (c) {
            const opt = $('<option>')
                .val(c.value)
                .text(c.text)
                .data('integrated', c.isIntegrated)
                .data('env', c.environment);
            select.append(opt);
        });

        // Re-apply any pre-selected value (Edit scenario)
        const preSelected = select.data('preselected');
        if (preSelected && preSelected !== '0') {
            select.val(preSelected);
            select.trigger('change');
        }
    }).fail(function () {
        console.warn('Could not load company list for dropdown.');
    });
});
