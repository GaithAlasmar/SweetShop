$(document).ready(function () {
    // Shared Offcanvas instance
    const cartOffcanvasEl = document.getElementById('cartOffcanvas');
    let bsOffcanvas = null;

    if (cartOffcanvasEl) {
        bsOffcanvas = new bootstrap.Offcanvas(cartOffcanvasEl);

        // Show overlay when cart opens
        cartOffcanvasEl.addEventListener('show.bs.offcanvas', function () {
            loadCart();
            showCartOverlay();
        });

        // Hide overlay when cart closes
        cartOffcanvasEl.addEventListener('hide.bs.offcanvas', function () {
            hideCartOverlay();
        });
    }

    // Reuseable AJAX config
    const ajaxHeaders = { "X-Requested-With": "XMLHttpRequest" };

    // Intercept "Add to Cart" clicks
    $(document).on('click', '.ajax-add-to-cart', function (e) {
        e.preventDefault();
        const btn = $(this);
        const url = btn.attr('href');

        if (btn.hasClass('disabled')) return;

        // Show loading state
        const originalText = btn.html();
        btn.html('<span class="spinner-border spinner-border-sm" role="status"></span>');
        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            headers: ajaxHeaders,
            success: function () {
                // 1. Refresh internal content
                loadCart(function () {
                    // 2. Open offcanvas ONLY after content is loaded to avoid "Empty" flash
                    if (bsOffcanvas) bsOffcanvas.show();
                });

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
        const btn = $(this);
        const url = btn.attr('href');

        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            headers: ajaxHeaders,
            success: function () {
                loadCart();
            },
            error: function (err) {
                console.error("Error removing from cart", err);
                btn.removeClass('disabled');
                alert('حدث خطأ أثناء حذف المنتج');
            }
        });
    });

    // Increase quantity
    $(document).on('click', '.ajax-increase-quantity', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('product-id');
        const url = '/ShoppingCart/IncreaseQuantity?productId=' + productId;

        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            headers: ajaxHeaders,
            success: function () {
                loadCart();
            },
            error: function (err) {
                console.error("Error increasing quantity", err);
                btn.removeClass('disabled');
            }
        });
    });

    // Decrease quantity
    $(document).on('click', '.ajax-decrease-quantity', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('product-id');
        const url = '/ShoppingCart/DecreaseQuantity?productId=' + productId;

        btn.addClass('disabled');

        $.ajax({
            url: url,
            type: 'GET',
            headers: ajaxHeaders,
            success: function () {
                loadCart();
            },
            error: function (err) {
                console.error("Error decreasing quantity", err);
                btn.removeClass('disabled');
            }
        });
    });
});

/**
 * Loads the cart partial view from the server
 * @param {Function} callback - Optional function to execute after load
 */
function loadCart(callback) {
    const container = $('#cart-offcanvas-content');

    $.ajax({
        url: '/ShoppingCart/GetCartPartial',
        type: 'GET',
        success: function (data) {
            container.html(data);
            updateCartBadge();
            if (typeof I18n !== 'undefined' && I18n.apply) {
                I18n.apply(I18n.getSaved());
            }
            if (typeof Currency !== 'undefined' && Currency.apply) {
                Currency.apply(Currency.getSaved());
            }
            if (typeof callback === 'function') callback();
        },
        error: function (xhr, status, error) {
            console.error("Cart load error:", status, error);
            container.html('<div class="text-center p-4"><p class="text-danger">حدث خطأ في تحميل السلة</p></div>');
        }
    });
}

function updateCartBadge() {
    $.ajax({
        url: '/ShoppingCart/GetCartCount',
        type: 'GET',
        success: function (count) {
            const badge = $('#cart-badge, #sticky-cart-badge');
            badge.text(count);
            // Optional: Hide badge if count is 0
            count > 0 ? badge.show() : badge.hide();
        },
        error: function (err) {
            console.log("Badge sync error (non-critical)");
        }
    });
}

/* ── Cart Overlay Helpers ── */
function showCartOverlay() {
    const ol = document.getElementById('cartOverlay');
    if (!ol) return;
    ol.style.display = 'block';
    // Force reflow so transition plays
    ol.offsetHeight;
    ol.classList.add('active');
}

function hideCartOverlay() {
    const ol = document.getElementById('cartOverlay');
    if (!ol) return;
    ol.classList.remove('active');
    // Remove from layout after transition ends
    ol.addEventListener('transitionend', function handler() {
        ol.style.display = 'none';
        ol.removeEventListener('transitionend', handler);
    });
}

/* Called when user clicks the backdrop */
function closeCartOverlay() {
    const cartEl = document.getElementById('cartOffcanvas');
    if (cartEl) {
        const bs = bootstrap.Offcanvas.getInstance(cartEl);
        if (bs) bs.hide();
    }
    hideCartOverlay();
}

