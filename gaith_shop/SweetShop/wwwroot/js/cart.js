$(document).ready(function () {
    // Load cart when offcanvas is opened
    var cartOffcanvas = document.getElementById('cartOffcanvas');
    cartOffcanvas.addEventListener('show.bs.offcanvas', function () {
        loadCart();
    });

    // Intercept "Add to Cart" clicks
    $(document).on('click', '.ajax-add-to-cart', function (e) {
        e.preventDefault();
        var btn = $(this);
        var url = btn.attr('href');

        // Show loading state on button (optional)
        var originalText = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جار الإضافة...');
        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET', // Or POST if changed in controller, defaulting to GET based on current implementation
            success: function (result) {
                // Determine if we need to redirect or if we can just update cart
                // Since controller returns RedirectToActionResult, fetch might follow it.
                // ideally, we update controller to return OK or check responseURL.
                // For now, let's assume successful add and reload cart.

                loadCart();

                // Open offcanvas
                var bsOffcanvas = new bootstrap.Offcanvas(cartOffcanvas);
                bsOffcanvas.show();

                // Reset button
                btn.html(originalText);
                btn.removeClass('disabled');
            },
            error: function (err) {
                console.error("Error adding to cart", err);
                btn.html(originalText);
                btn.removeClass('disabled');
            }
        });
    });

    // Intercept "Remove from Cart" clicks
    $(document).on('click', '.ajax-remove-from-cart', function (e) {
        e.preventDefault();
        var btn = $(this);
        var url = btn.attr('href');

        // Show loading state
        var originalHtml = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status"></span>');
        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                // Reload cart to show updated items
                loadCart();

                // Reset button (in case it's still visible)
                btn.html(originalHtml);
                btn.removeClass('disabled');
            },
            error: function (err) {
                console.error("Error removing from cart", err);
                btn.html(originalHtml);
                btn.removeClass('disabled');
                alert('حدث خطأ أثناء حذف المنتج');
            }
        });
    });

    // Increase quantity
    $(document).on('click', '.ajax-increase-quantity', function (e) {
        e.preventDefault();
        var btn = $(this);
        var productId = btn.data('product-id');
        var url = '/ShoppingCart/IncreaseQuantity?productId=' + productId;

        // Show loading state
        var originalHtml = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status"></span>');
        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                loadCart();
                btn.html(originalHtml);
                btn.removeClass('disabled');
            },
            error: function (err) {
                console.error("Error increasing quantity", err);
                btn.html(originalHtml);
                btn.removeClass('disabled');
                alert('حدث خطأ أثناء زيادة الكمية');
            }
        });
    });

    // Decrease quantity
    $(document).on('click', '.ajax-decrease-quantity', function (e) {
        e.preventDefault();
        var btn = $(this);
        var productId = btn.data('product-id');
        var url = '/ShoppingCart/DecreaseQuantity?productId=' + productId;

        // Show loading state
        var originalHtml = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status"></span>');
        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                loadCart();
                btn.html(originalHtml);
                btn.removeClass('disabled');
            },
            error: function (err) {
                console.error("Error decreasing quantity", err);
                btn.html(originalHtml);
                btn.removeClass('disabled');
                alert('حدث خطأ أثناء تقليل الكمية');
            }
        });
    });
});

function loadCart() {
    $.ajax({
        url: '/ShoppingCart/GetCartPartial',
        type: 'GET',
        success: function (data) {
            $('#cart-offcanvas-content').html(data);
            updateCartBadge(); // Update badge count whenever cart is loaded
            // Update badge count if needed (requires parsing data or separate API)
            // Simple badge update logic could be added here if partial includes it or specific API exists
        },
        error: function (xhr, status, error) {
            console.error("Cart load error:", status, error);
            $('#cart-offcanvas-content').html('<div class="text-center p-3"><i class="bi bi-exclamation-circle text-danger fs-1"></i><p class="text-danger mt-2">حدث خطأ: ' + xhr.status + ' ' + error + '</p><p class="text-muted small">يرجى إعادة تشغيل السيرفر لتحديث التغييرات.</p></div>');
        }
    });
}

function updateCartBadge() {
    $.ajax({
        url: '/ShoppingCart/GetCartCount',
        type: 'GET',
        success: function (count) {
            $('#cart-badge').text(count);
            $('#sticky-cart-badge').text(count);
        },
        error: function (err) {
            console.error("Error updating cart badge", err);
        }
    });
}
