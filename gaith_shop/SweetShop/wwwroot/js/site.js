// Sticky Header: يُفعَّل بعد 150px من التمرير
window.addEventListener('scroll', function () {
    var dropdown = document.getElementById('stickyCategoriesDropdown');
    var isDropdownOpen = dropdown && dropdown.getAttribute('aria-expanded') === 'true';

    if (window.scrollY > 150) {
        document.body.classList.add('sticky-mode');
    } else {
        if (!isDropdownOpen) {
            document.body.classList.remove('sticky-mode');
        }
    }
});

// ── AOS — Animate On Scroll ────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 700,
            easing: 'ease-out-quart',
            once: true,
            offset: 60,
            delay: 0,
        });
    }
});

// ── i18n: Restore saved language preference on every page load ────────────
document.addEventListener('DOMContentLoaded', function () {
    if (typeof I18n !== 'undefined') {
        var savedLang = I18n.getSaved(); // reads localStorage, defaults to 'ar'
        I18n.apply(savedLang);
    }
});

// ── Language Switcher — toggle open / close ────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    var switcher = document.getElementById('langSwitcher');
    var btn = document.getElementById('langSwitcherBtn');
    var dropdown = document.getElementById('langDropdown');

    if (!switcher || !btn || !dropdown) return;

    btn.addEventListener('click', function (e) {
        e.stopPropagation();
        var isOpen = switcher.classList.toggle('is-open');
        btn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    });

    document.addEventListener('click', function (e) {
        if (!switcher.contains(e.target)) {
            switcher.classList.remove('is-open');
            btn.setAttribute('aria-expanded', 'false');
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            switcher.classList.remove('is-open');
            btn.setAttribute('aria-expanded', 'false');
            btn.focus();
        }
    });

    // ── Currency Switcher — toggle open / close ────────────────────────────────
    var cSwitcher = document.getElementById('currencySwitcher');
    var cBtn = document.getElementById('currencyDropdownBtn');

    if (cSwitcher && cBtn) {
        cBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            var isOpen = cSwitcher.classList.toggle('is-open');
            cBtn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        });

        document.addEventListener('click', function (e) {
            if (!cSwitcher.contains(e.target)) {
                cSwitcher.classList.remove('is-open');
                cBtn.setAttribute('aria-expanded', 'false');
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                cSwitcher.classList.remove('is-open');
                cBtn.setAttribute('aria-expanded', 'false');
                cBtn.focus();
            }
        });
    }
});

/**
 * switchLanguage(optionEl)
 * ─────────────────────────
 * Called via onclick="switchLanguage(this)" on each language option button.
 *
 * Attributes read from the button element:
 *   data-lang   → 'ar' | 'en'
 *   data-action → (optional) ASP.NET controller route URL
 *                 e.g. "/Home/SetLanguage"
 *                 When present the browser navigates there with ?lang=&returnUrl=
 *                 so the CookieRequestCultureProvider can persist the choice
 *                 server-side, making every Razor-rendered response use the
 *                 correct CultureInfo.
 */
function switchLanguage(optionEl) {
    var lang = optionEl.getAttribute('data-lang');
    var action = optionEl.getAttribute('data-action');

    // 1. Apply translations + update <html lang/dir> + save to localStorage
    if (typeof I18n !== 'undefined') {
        I18n.apply(lang);
    } else {
        // Fallback (i18n.js not yet loaded — should not happen in normal flow)
        document.documentElement.lang = lang;
        document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
    }

    // Re-apply currency (to update currency UI text based on new language e.g Jordan -> الأردن)
    if (typeof Currency !== 'undefined') {
        Currency.apply(Currency.getSaved());
    }

    // 2. Close the dropdown
    var switcher = document.getElementById('langSwitcher');
    var btn = document.getElementById('langSwitcherBtn');
    if (switcher) switcher.classList.remove('is-open');
    if (btn) btn.setAttribute('aria-expanded', 'false');

    // 3. Optionally persist culture server-side via ASP.NET localization route.
    //    The controller writes a .AspNetCore.Culture cookie, which the
    //    CookieRequestCultureProvider reads on every subsequent request so
    //    server-rendered content (product names, prices, validation messages)
    //    also uses the correct CultureInfo.
    if (action && action !== '' && action !== '#') {
        var returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
        window.location.href = action + '?lang=' + lang + '&returnUrl=' + returnUrl;
    }
}

/**
 * switchCurrencyUI(optionEl)
 * ─────────────────────────
 * Called to close the custom currency dropdown after a selection is made.
 */
function switchCurrencyUI(optionEl) {
    if (typeof Currency !== 'undefined') {
        Currency.apply(optionEl.getAttribute('data-currency'));
    }

    var cSwitcher = document.getElementById('currencySwitcher');
    var cBtn = document.getElementById('currencyDropdownBtn');
    if (cSwitcher) cSwitcher.classList.remove('is-open');
    if (cBtn) cBtn.setAttribute('aria-expanded', 'false');
}
