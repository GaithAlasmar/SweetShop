/**
 * SweetShop — Client-Side Currency Converter
 * Handles dynamic conversion to USD, AED, SAR from base JOD.
 */
window.Currency = (function () {
    var STORAGE_KEY = 'sweetshop_currency';
    var defaultCurrency = 'JOD';

    var rates = {
        'JOD': 1.0,
        'USD': 1.41,
        'AED': 5.18,
        'SAR': 5.29
    };

    var symbols = {
        'JOD': { ar: 'د.أ', en: 'JOD' },
        'USD': { ar: '$', en: '$' },
        'AED': { ar: 'د.إ', en: 'AED' },
        'SAR': { ar: 'ر.س', en: 'SAR' }
    };

    function apply(code) {
        if (!rates[code]) code = defaultCurrency;
        try { localStorage.setItem(STORAGE_KEY, code); } catch (e) { }

        var rate = rates[code];
        var lang = document.documentElement.lang || 'ar';

        var symbol = symbols[code][lang] || symbols[code]['en'];

        document.querySelectorAll('[data-price]').forEach(function (el) {
            var rawPrice = parseFloat(el.getAttribute('data-price'));
            if (isNaN(rawPrice)) return;

            var converted = rawPrice * rate;
            var formatted = converted.toFixed(2);

            if (lang === 'ar') {
                el.innerText = formatted + ' ' + symbol;
            } else {
                el.innerText = symbol + formatted;
            }
        });

        updateHeaderUI(code, lang);
    }

    function updateHeaderUI(code, lang) {
        var names = {
            'JOD': { ar: 'الأردن', en: 'Jordan', flag: 'jo', curAr: 'دينار أردني', curEn: 'JOD' },
            'USD': { ar: 'الولايات المتحدة', en: 'United States', flag: 'us', curAr: 'دولار أمريكي', curEn: 'USD' },
            'AED': { ar: 'الإمارات', en: 'UAE', flag: 'ae', curAr: 'درهم إماراتي', curEn: 'AED' },
            'SAR': { ar: 'السعودية', en: 'Saudi Arabia', flag: 'sa', curAr: 'ريال سعودي', curEn: 'SAR' }
        };

        var info = names[code];
        if (!info) return;

        var countryNameEl = document.getElementById('selectedCurrencyName');
        var curNameEl = document.getElementById('selectedCurrencyCode');
        var flagEl = document.getElementById('selectedCurrencyFlag');

        if (countryNameEl) {
            countryNameEl.innerText = lang === 'ar' ? info.ar : info.en;
            // Remove data-i18n so it doesn't get overwritten by the static i18n dictionary
            countryNameEl.removeAttribute('data-i18n');
        }
        if (curNameEl) {
            curNameEl.innerText = lang === 'ar' ? info.curAr : info.curEn;
            curNameEl.removeAttribute('data-i18n');
        }
        if (flagEl) {
            flagEl.src = 'https://flagcdn.com/w20/' + info.flag + '.png';
        }

        document.querySelectorAll('.currency-option').forEach(function (btn) {
            if (btn.getAttribute('data-currency') === code) {
                btn.style.fontWeight = 'bold';
                btn.style.backgroundColor = 'rgba(26, 140, 176, 0.1)';
            } else {
                btn.style.fontWeight = 'normal';
                btn.style.backgroundColor = 'transparent';
            }
        });
    }

    function init() {
        var saved = defaultCurrency;
        try { saved = localStorage.getItem(STORAGE_KEY) || defaultCurrency; } catch (e) { }
        apply(saved);
    }

    return {
        init: init,
        apply: apply,
        getSaved: function () {
            try { return localStorage.getItem(STORAGE_KEY) || defaultCurrency; } catch (e) { return defaultCurrency; }
        }
    };
})();

document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        Currency.init();
    }, 50);
});
