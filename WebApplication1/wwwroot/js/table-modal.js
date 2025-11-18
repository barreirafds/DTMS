// Table Modal JavaScript
// Manages the functionality to add products to the table modal

document.addEventListener('DOMContentLoaded', function() {
    const tableModal = document.getElementById('tableModal');
    const productsTableBody = document.getElementById('productsTableBody');
    const productSelect = document.getElementById('productSelect');
    const productQuantity = document.getElementById('productQuantity');
    const addProductBtn = document.getElementById('addProductBtn');
    const saveOrderBtn = document.getElementById('saveOrderBtn');
    
    // Array to store added products
    let addedProducts = [];
    let currentTableId = null;

    // When modal opens
    tableModal.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget;
        
        currentTableId = button.getAttribute('data-table-id');
        const tableNumber = button.getAttribute('data-table-number');
        const tableSeats = button.getAttribute('data-table-seats');
        const tableStatus = button.getAttribute('data-table-status');

        // Update table information in modal
        document.getElementById('modalTableNumber').textContent = tableNumber || '-';
        document.getElementById('modalTableSeats').textContent = tableSeats || '-';
        
        const statusBadge = document.getElementById('modalTableStatus');
        statusBadge.textContent = tableStatus || 'Available';
        statusBadge.className = 'badge ' + getStatusBadgeClass(tableStatus);
        
        // Clear previous products
        addedProducts = [];
        renderProductsTable();
    });

    // When modal closes
    tableModal.addEventListener('hidden.bs.modal', function () {
        // Reset form
        productSelect.value = '';
        productQuantity.value = '1';
        addedProducts = [];
        currentTableId = null;
    });

    // Add product
    addProductBtn.addEventListener('click', function() {
        const selectedOption = productSelect.options[productSelect.selectedIndex];
        
        if (!selectedOption.value) {
            showAlert('Please select a product.', 'warning');
            return;
        }

        const productId = parseInt(selectedOption.value);
        const productName = selectedOption.getAttribute('data-name');
        const productPrice = parseFloat(selectedOption.getAttribute('data-price'));
        const productCategory = selectedOption.getAttribute('data-category');
        const quantity = parseInt(productQuantity.value) || 1;

        if (quantity <= 0) {
            showAlert('Quantity must be greater than zero.', 'warning');
            return;
        }

        // Check if product was already added
        const existingProductIndex = addedProducts.findIndex(p => p.id === productId);
        
        if (existingProductIndex >= 0) {
            // If exists, update quantity
            addedProducts[existingProductIndex].quantity += quantity;
        } else {
            // Add new product
            addedProducts.push({
                id: productId,
                name: productName,
                price: productPrice,
                category: productCategory,
                quantity: quantity
            });
        }

        // Render table and reset form
        renderProductsTable();
        productSelect.value = '';
        productQuantity.value = '1';
        
        showAlert('Product added successfully!', 'success');
    });

    // Remove product
    productsTableBody.addEventListener('click', function(e) {
        if (e.target.classList.contains('remove-product-btn')) {
            const productId = parseInt(e.target.getAttribute('data-product-id'));
            addedProducts = addedProducts.filter(p => p.id !== productId);
            renderProductsTable();
            showAlert('Product removed.', 'info');
        }
    });

    // Save order
    saveOrderBtn.addEventListener('click', function() {
        if (addedProducts.length === 0) {
            showAlert('Please add at least one product before saving.', 'warning');
            return;
        }

        // Here you can add logic to send data to the server
        const orderData = {
            tableId: currentTableId,
            products: addedProducts,
            total: calculateTotal()
        };

        console.log('Order to save:', orderData);
        
        // For now, just show a message
        showAlert('Order saved successfully! (Feature to be implemented)', 'success');
        
        // Close modal after a brief delay
        setTimeout(() => {
            const modal = bootstrap.Modal.getInstance(tableModal);
            if (modal) {
                modal.hide();
            }
        }, 1500);
    });

    // Render products table
    function renderProductsTable() {
        if (addedProducts.length === 0) {
            productsTableBody.innerHTML = `
                <tr id="emptyRow">
                    <td colspan="6" class="text-center text-muted">
                        No products added yet
                    </td>
                </tr>
            `;
        } else {
            productsTableBody.innerHTML = addedProducts.map(product => {
                const total = (product.price * product.quantity).toFixed(2);
                return `
                    <tr>
                        <td>${escapeHtml(product.name)}</td>
                        <td><span class="badge bg-secondary">${escapeHtml(product.category)}</span></td>
                        <td>${product.quantity}</td>
                        <td>$${product.price.toFixed(2)}</td>
                        <td><strong>$${total}</strong></td>
                        <td>
                            <button class="btn btn-sm btn-danger remove-product-btn" 
                                    data-product-id="${product.id}"
                                    title="Remove">
                                <i class="bi bi-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
            }).join('');
        }

        // Update total
        document.getElementById('totalAmount').textContent = '$' + calculateTotal().toFixed(2);
    }

    // Calculate total
    function calculateTotal() {
        return addedProducts.reduce((sum, product) => {
            return sum + (product.price * product.quantity);
        }, 0);
    }

    // Get status badge class
    function getStatusBadgeClass(status) {
        const s = (status || 'Available').toLowerCase();
        if (s === 'available') return 'bg-success';
        else if (s === 'occupied') return 'bg-danger';
        else if (s === 'reserved') return 'bg-warning';
        else return 'bg-secondary';
    }

    // Show temporary alert
    function showAlert(message, type) {
        // Remove previous alerts
        const existingAlert = document.querySelector('.temp-alert');
        if (existingAlert) {
            existingAlert.remove();
        }

        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show temp-alert`;
        alertDiv.setAttribute('role', 'alert');
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        // Insert at the beginning of modal body
        const modalBody = tableModal.querySelector('.modal-body');
        modalBody.insertBefore(alertDiv, modalBody.firstChild);

        // Remove automatically after 3 seconds
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 3000);
    }

    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
});

