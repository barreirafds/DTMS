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
        
        // No alert for adding product - cleaner UX
    });

    // Remove product
    productsTableBody.addEventListener('click', function(e) {
        if (e.target.classList.contains('remove-product-btn')) {
            const productId = parseInt(e.target.getAttribute('data-product-id'));
            addedProducts = addedProducts.filter(p => p.id !== productId);
            renderProductsTable();
            // Product removed silently
        }
    });

    // Save order - Refactored and robust
    saveOrderBtn.addEventListener('click', async function() {
        if (addedProducts.length === 0) {
            showAlert('Please add at least one product before saving.', 'warning');
            return;
        }

        if (!currentTableId) {
            showAlert('Table ID is missing.', 'danger');
            return;
        }

        // Disable button to prevent double submission
        saveOrderBtn.disabled = true;
        const originalButtonText = saveOrderBtn.innerHTML;
        saveOrderBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Saving...';

        // Prepare order data with camelCase property names
        const orderData = {
            tableId: parseInt(currentTableId),
            items: addedProducts.map(product => ({
                productId: product.id,
                quantity: product.quantity,
                price: product.price
            }))
        };

        console.log('Order data to send:', JSON.stringify(orderData, null, 2));

        try {
            // Determine the current page to use the correct endpoint
            const currentPath = window.location.pathname;
            const handlerPath = currentPath.includes('/Dashboard') ? '/Dashboard?handler=SaveOrder' : '/Index?handler=SaveOrder';

            console.log('Sending to:', handlerPath);

            const response = await fetch(handlerPath, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(orderData)
            });

            console.log('Response status:', response.status);
            console.log('Response ok:', response.ok);

            // Read response
            let result;
            const responseText = await response.text();
            console.log('Response text:', responseText);
            
            // Handle empty response
            if (!responseText || responseText.trim() === '') {
                console.error('Empty response from server');
                showAlert(`Server error: ${response.status} - Empty response. Please check server logs.`, 'danger');
                saveOrderBtn.disabled = false;
                saveOrderBtn.innerHTML = originalButtonText;
                return;
            }
            
            try {
                result = JSON.parse(responseText);
                console.log('Parsed result:', result);
            } catch (parseError) {
                console.error('Failed to parse response:', parseError);
                console.error('Response text was:', responseText);
                showAlert(`Server error: ${response.status} - ${responseText.substring(0, 200) || 'Invalid response format'}`, 'danger');
                saveOrderBtn.disabled = false;
                saveOrderBtn.innerHTML = originalButtonText;
                return;
            }

            if (response.ok && result.success) {
                showAlert(result.message || 'Order saved successfully!', 'success');
                
                // Clear products and close modal after a brief delay
                setTimeout(() => {
                    addedProducts = [];
                    renderProductsTable();
                    const modal = bootstrap.Modal.getInstance(tableModal);
                    if (modal) {
                        modal.hide();
                    }
                    saveOrderBtn.disabled = false;
                    saveOrderBtn.innerHTML = originalButtonText;
                }, 1500);
            } else {
                const errorMessage = result.message || result.error || `Error ${response.status}: Failed to save order`;
                showAlert(errorMessage, 'danger');
                saveOrderBtn.disabled = false;
                saveOrderBtn.innerHTML = originalButtonText;
            }
        } catch (error) {
            console.error('Error saving order:', error);
            showAlert('An error occurred while saving the order: ' + (error.message || 'Unknown error'), 'danger');
            saveOrderBtn.disabled = false;
            saveOrderBtn.innerHTML = originalButtonText;
        }
    });

    // Render products table
    function renderProductsTable() {
        if (addedProducts.length === 0) {
            productsTableBody.innerHTML = `
                <tr id="emptyRow">
                    <td colspan="5" class="text-center text-muted py-3">
                        No products added
                    </td>
                </tr>
            `;
        } else {
            productsTableBody.innerHTML = addedProducts.map(product => {
                const total = (product.price * product.quantity).toFixed(2);
                return `
                    <tr>
                        <td>${escapeHtml(product.name)}</td>
                        <td class="text-center">${product.quantity}</td>
                        <td class="text-end">${product.price.toFixed(2)}€</td>
                        <td class="text-end">${total}€</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-danger remove-product-btn" 
                                    data-product-id="${product.id}"
                                    title="Remove">
                                ×
                            </button>
                        </td>
                    </tr>
                `;
            }).join('');
        }

        // Update total
        document.getElementById('totalAmount').textContent = calculateTotal().toFixed(2) + '€';
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
