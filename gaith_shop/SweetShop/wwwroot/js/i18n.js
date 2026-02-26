/**
 * SweetShop — Client-Side i18n Module
 * =====================================
 * Usage: I18n.apply('ar') | I18n.apply('en')
 *
 * How it works:
 *   1. Every translatable DOM element carries a  data-i18n="key"  attribute.
 *   2. I18n.apply(lang) loops all such elements and swaps their text from the
 *      dictionary below.  Inputs/textareas also swap their placeholder via
 *      data-i18n-placeholder="key".
 *   3. The chosen language is stored in localStorage so every page reload
 *      restores the preference.
 *   4. <html lang> and <html dir> are updated automatically.
 *
 * Backend wire-up (ASP.NET):
 *   The language switcher button carries  data-action="/Home/SetLanguage".
 *   After the DOM update, switchLanguage() (in site.js) redirects the browser
 *   through that route so ASP.NET's CookieRequestCultureProvider persists the
 *   culture for server-rendered content (product names, prices, etc.).
 */

window.I18n = (function () {

    /* ── 1.  Translation Dictionary ─────────────────────────────────────── */
    var t = {

        /* ── Top Bar ── */
        'topbar.email.label': { ar: 'البريد :', en: 'Email:' },
        'topbar.phone.label': { ar: 'الهاتف :', en: 'Phone:' },
        'topbar.currency': { ar: 'دينار أردني', en: 'Jordanian Dinar' },
        'topbar.country': { ar: 'الأردن', en: 'Jordan' },
        'topbar.lang': { ar: 'العربية', en: 'English' },
        'currency.jod': { ar: 'د.أ', en: 'JOD' },

        /* ── Main Header ── */
        'header.account': { ar: 'حسابي', en: 'Account' },
        'header.cart': { ar: 'عربتي', en: 'My Cart' },
        'header.search.ph': { ar: 'ابحث عن حلويات، كنافة، كعك...', en: 'Search for sweets, kunafa, cake...' },
        'header.search.btn': { ar: 'بحث', en: 'Search' },

        /* ── Category Nav Links ── */
        'nav.all': { ar: 'جميع الأصناف', en: 'All Categories' },
        'nav.arabic': { ar: 'حلويات عربية', en: 'Arabic Sweets' },
        'nav.kunafa': { ar: 'كنافة', en: 'Kunafa' },
        'nav.western': { ar: 'الحلويات الغربية', en: 'Western Sweets' },
        'nav.chocolate': { ar: 'الشوكلاتة والهدايا', en: 'Chocolate & Gifts' },
        'nav.special': { ar: 'عبوات خاصة', en: 'Special Packs' },
        'nav.ice': { ar: 'بوظة', en: 'Ice Cream' },
        'nav.orders': { ar: 'طلبيات', en: 'Orders' },
        'nav.branches': { ar: 'فروعنا', en: 'Our Branches' },
        'nav.admin': { ar: 'لوحة التحكم', en: 'Dashboard' },

        /* ── Hero Carousel — Slide 1 ── */
        'hero.s1.tag': { ar: '✦ حلويات الاسمر — منذ ١٩٩٥', en: '✦ Al-Asmar Sweets — Since 1995' },
        'hero.s1.title': { ar: 'أصالة الحلويات', en: 'The Authenticity of' },
        'hero.s1.title.gold': { ar: 'العربية', en: 'Arabic Sweets' },
        'hero.s1.sub': {
            ar: 'من الكنافة النابلسية إلى البقلاوة الدمشقية\nنصنع السعادة بكل قطعة',
            en: 'From Nabulsi Kunafa to Damascene Baklava\nWe craft happiness in every piece'
        },
        'hero.s1.btn1': { ar: 'تصفح القائمة', en: 'Browse Menu' },
        'hero.s1.btn2': { ar: 'فروعنا', en: 'Our Branches' },

        /* ── Hero Carousel — Slide 2 ── */
        'hero.s2.tag': { ar: '✦ كنافة طازجة يومياً', en: '✦ Fresh Kunafa Daily' },
        'hero.s2.title': { ar: 'الكنافة النابلسية', en: 'Nabulsi Kunafa' },
        'hero.s2.title.gold': { ar: 'الأصيلة', en: 'The Original' },
        'hero.s2.sub': {
            ar: 'محشوّة بالجبن الطازج والقطر العربي الشهي\nتُصنع أمامك كل يوم بحب واهتمام',
            en: 'Stuffed with fresh cheese and sweet Arabic syrup\nMade fresh daily with love and care'
        },
        'hero.s2.btn1': { ar: 'اطلب الآن', en: 'Order Now' },
        'hero.s2.btn2': { ar: 'تفاصيل', en: 'Details' },

        /* ── Hero Carousel — Slide 3 ── */
        'hero.s3.tag': { ar: '✦ هدايا المناسبات الخاصة', en: '✦ Special Occasion Gifts' },
        'hero.s3.title': { ar: 'هدايا تُعبّر', en: 'Gifts That' },
        'hero.s3.title.gold': { ar: 'عن مشاعرك', en: 'Express Your Feelings' },
        'hero.s3.sub': {
            ar: 'أطباق هدايا فاخرة للأعياد والمناسبات\nبتغليف احترافي يُبهر من تحبهم',
            en: 'Luxury gift trays for holidays and occasions\nWith professional packaging that amazes your loved ones'
        },
        'hero.s3.btn1': { ar: 'تصفح الهدايا', en: 'Browse Gifts' },
        'hero.s3.btn2': { ar: 'اطلب خاص', en: 'Special Order' },

        /* ── Homepage — Browse Section ── */
        'home.browseTitle': { ar: 'تصفح جميع الأقسام', en: 'Browse All Categories' },
        'home.browseSub': { ar: 'اكتشف تشكيلتنا الواسعة من الحلويات والهدايا', en: 'Discover our wide selection of sweets and gifts' },

        /* ── Product Card shared UI ── */
        'card.addToCart': { ar: 'أضف إلى السلة', en: 'Add to Cart' },
        'card.viewDetails': { ar: 'عرض التفاصيل', en: 'View Details' },
        'card.addToCartShort': { ar: 'أضف للسلة', en: 'Add to Cart' },
        'card.unitPrice': { ar: '/ قطعة', en: '/ piece' },

        /* ── Product List Page ── */
        'list.countSuffix': { ar: ' منتج متاح في هذا القسم', en: ' products available' },
        'list.emptyTitle': { ar: 'لا توجد منتجات', en: 'No Products Found' },
        'list.emptyDesc': { ar: 'عذراً، لا توجد منتجات متاحة في هذا القسم حالياً.', en: 'Sorry, no products are currently available in this category.' },
        'list.backHome': { ar: 'العودة للرئيسية', en: 'Back to Home' },

        /* ── Search Page ── */
        'search.pageTitle': { ar: 'البحث في المنتجات', en: 'Search Products' },
        'search.enterQuery': { ar: 'أدخل كلمة البحث', en: 'Enter a search term' },
        'search.enterQueryDesc': { ar: 'استخدم شريط البحث للعثور على منتجك المفضل', en: 'Use the search bar to find your favourite product' },
        'search.noResults': { ar: 'لا توجد نتائج', en: 'No Results Found' },
        'search.browseAll': { ar: 'تصفح جميع المنتجات', en: 'Browse All Products' },
        'search.tipSpelling': { ar: 'تحقق من الإملاء', en: 'Check your spelling' },
        'search.tipGeneral': { ar: 'استخدم كلمات أكثر عمومية', en: 'Try more general terms' },
        'search.tipFewer': { ar: 'قلّل عدد الكلمات', en: 'Use fewer words' },

        /* ── Cart Partial (Side Drawer) ── */
        'cart.title': { ar: 'سلة المشتريات', en: 'Shopping Cart' },
        'cart.total': { ar: 'المجموع', en: 'Total' },
        'cart.checkout': { ar: 'إتمام الطلب', en: 'Checkout' },
        'cart.viewFull': { ar: 'عرض السلة كاملة', en: 'View Full Cart' },
        'cart.emptyMsg': { ar: 'لم تقم بإضافة حلويات بعد', en: "You haven't added any sweets yet" },
        'cart.emptySmall': { ar: 'ابدأ بتصفح منتجاتنا اللذيذة', en: 'Start browsing our delicious products' },
        'cart.browse': { ar: 'تصفح القائمة', en: 'Browse Menu' },
        'cart.unitPiece': { ar: '/ قطعة', en: '/ piece' },

        /* ── Shopping Cart Index Page ── */
        'cartpage.title': { ar: 'سلة المشتريات الخاصة بك', en: 'Your Shopping Cart' },
        'cartpage.qty': { ar: 'الكمية', en: 'Qty' },
        'cartpage.sweet': { ar: 'الحلوى', en: 'Item' },
        'cartpage.price': { ar: 'السعر', en: 'Price' },
        'cartpage.subtotal': { ar: 'المجموع الفرعي', en: 'Subtotal' },
        'cartpage.total': { ar: 'المجموع:', en: 'Total:' },
        'cartpage.checkout': { ar: 'إتمام الشراء الآن!', en: 'Proceed to Checkout!' },
        'cartpage.remove': { ar: 'إزالة', en: 'Remove' },

        /* ── Branches Page ── */
        'branches.heroTitle': { ar: 'فروعنا', en: 'Our Branches' },
        'branches.heroSub': { ar: 'تفضلوا بزيارتنا في أقرب فرع إليكم', en: 'Come visit us at the nearest branch to you' },
        'branches.openNow': { ar: 'مفتوح الآن', en: 'Open Now' },
        'branches.location': { ar: 'الموقع', en: 'Location' },
        'branches.call': { ar: 'اتصل بنا', en: 'Call Us' },

        /* ── Login Page ── */
        'login.newCustomer': { ar: 'عميل جديد؟', en: 'New Customer?' },
        'login.newSubtitle': { ar: 'التسجيل في حلويات الاسمر سريع وسهل!', en: 'Registering with Al-Asmar Sweets is quick and easy!' },
        'login.benefit1': { ar: 'إتمام الطلب بشكل أسرع', en: 'Faster checkout' },
        'login.benefit2': { ar: 'حفظ عناوين للشحن المتعدد', en: 'Save multiple shipping addresses' },
        'login.benefit3': { ar: 'تتبع طلباتك بسهولة', en: 'Easily track your orders' },
        'login.createAccount': { ar: 'إنشاء حساب', en: 'Create Account' },
        'login.existingTitle': { ar: 'عميل حالي', en: 'Existing Customer' },
        'login.welcomeBack': { ar: 'مرحباً بك مجدداً', en: 'Welcome back' },
        'login.signIn': { ar: 'تسجيل الدخول', en: 'Sign In' },
        'login.forgotPw': { ar: 'نسيت كلمة المرور؟', en: 'Forgot your password?' },
        'login.brandTitle': { ar: 'حلويات الاسمر', en: 'Al-Asmar Sweets' },
        'login.registerNew': { ar: 'تسجيل كمستخدم جديد', en: 'Register as a new user' },
        'login.resendConf': { ar: 'إعادة إرسال تأكيد البريد الإلكتروني', en: 'Resend email confirmation' },

        /* ── Register Page ── */
        'register.title': { ar: 'إنشاء حساب', en: 'Create Account' },
        'register.subtitle': { ar: 'انضم إلى عائلة حلويات الاسمر وتمتع بمزايا حصرية', en: 'Join the Al-Asmar Sweets family and enjoy exclusive benefits' },
        'register.submit': { ar: 'إنشاء حساب', en: 'Create Account' },
        'register.haveAccount': { ar: 'لديك حساب بالفعل؟', en: 'Already have an account?' },
        'register.signIn': { ar: 'تسجيل الدخول', en: 'Sign In' },

        /* ── Cart Page ── */
        'cartpage.title': { ar: 'سلة المشتريات الخاصة بك', en: 'Your Shopping Cart' },
        'cartpage.qty': { ar: 'الكمية', en: 'Quantity' },
        'cartpage.sweet': { ar: 'المنتج', en: 'Product' },
        'cartpage.price': { ar: 'السعر', en: 'Price' },
        'cartpage.subtotal': { ar: 'المجموع', en: 'Subtotal' },
        'cartpage.remove': { ar: 'إزالة', en: 'Remove' },
        'cartpage.total': { ar: 'الإجمالي:', en: 'Total:' },
        'cartpage.summaryTitle': { ar: 'ملخص الطلب', en: 'Order Summary' },
        'cartpage.checkout': { ar: 'إتمام الشراء الآن!', en: 'Proceed to Checkout!' },
        'cartpage.secureCheckout': { ar: 'دفع آمن وموثوق بنسبة 100%', en: '100% Secure Checkout' },
        'cartpage.emptyTitle': { ar: 'سلة المشتريات فارغة', en: 'Your Cart is Empty' },
        'cartpage.emptyDesc': { ar: 'لم تقم بإضافة أي حلويات بعد، تصفح منتجاتنا اللذيذة وابدأ التسوق!', en: 'You haven\'t added any sweets yet. Browse our delicious products and start shopping!' },
        'cartpage.browseSweets': { ar: 'تصفح الحلويات', en: 'Browse Sweets' },

        /* ── Payment Gateway Page ── */
        'payment.secureTitle': { ar: 'بوابة الدفع الآمنة', en: 'Secure Payment Gateway' },
        'payment.mockDesc': { ar: 'تجربة دفع آمنة (Mock Environment)', en: 'Secure Payment Experience (Mock Environment)' },
        'payment.orderAmount': { ar: 'مبلغ الطلب:', en: 'Order Amount:' },
        'payment.cardDetails': { ar: 'تفاصيل البطاقة البنكية', en: 'Credit Card Details' },
        'payment.cardName': { ar: 'الاسم على البطاقة', en: 'Name on Card' },
        'payment.cardNumber': { ar: 'رقم البطاقة', en: 'Card Number' },
        'payment.expiry': { ar: 'تاريخ الانتهاء', en: 'Expiry Date' },
        'payment.payNow': { ar: 'دفع الآن', en: 'Pay Now' },
        'payment.cancelBtn': { ar: 'إلغاء والعودة للمتجر', en: 'Cancel and Return to Store' },

        'payment.successTitle': { ar: 'تم الدفع بنجاح!', en: 'Payment Successful!' },
        'payment.successDesc': { ar: 'شكرًا لك. لقد تلقينا طلبك بنجاح وتم تأكيد عملية الدفع.', en: 'Thank you. We have successfully received your order and payment is confirmed.' },
        'payment.pendingTitle': { ar: 'جاري معالجة الدفع...', en: 'Processing Payment...' },
        'payment.pendingDesc': { ar: 'طلبك قيد الانتظار حالياً، نحن بانتظار تأكيد بوابة الدفع. سيتم تحديث حالة الطلب تلقائياً قريباً.', en: 'Your order is currently pending, awaiting confirmation from the payment gateway. The exact status will update soon.' },
        'payment.failedTitle': { ar: 'فشلت عملية الدفع', en: 'Payment Failed' },
        'payment.failedDesc': { ar: 'عذراً، لم نتمكن من معالجة الدفع. يرجى المحاولة مرة أخرى أو استخدام بطاقة مختلفة.', en: 'Sorry, we could not process your payment. Please try again or use a different card.' },

        'payment.orderNo': { ar: 'رقم الطلب:', en: 'Order No:' },
        'payment.totalAmount': { ar: 'المبلغ الإجمالي:', en: 'Total Amount:' },
        'payment.transId': { ar: 'رقم المعاملة (المرجع):', en: 'Transaction ID (Ref):' },
        'payment.tryAgain': { ar: 'المحاولة مرة أخرى', en: 'Try Again' },
        'payment.returnHome': { ar: 'العودة للرئيسية', en: 'Return Home' },

        'payment.cancelHeading': { ar: 'تم إلغاء عملية الدفع', en: 'Payment Cancelled' },
        'payment.cancelMessage': { ar: 'لم يتم سحب أي مبالغ من بطاقتك والمشتروات لا تزال في سلتك في حال أردت استكمال الشراء لاحقاً.', en: 'No funds were deducted from your card, and the items remain in your cart to complete the purchase later.' },
        'payment.resumePayment': { ar: 'استكمال الدفع', en: 'Resume Payment' },
        'payment.returnCart': { ar: 'العودة للسلة', en: 'Return to Cart' },

        /* ── Orders / Special Order Create Page ── */
        'orders.heroTitle': { ar: 'طلبيات خاصة', en: 'Special Orders' },
        'orders.heroSub': { ar: 'أنشئ طلبك المخصص بالكمية والمنتجات التي تناسبك', en: 'Create your custom order with the quantities and products that suit you' },
        'orders.customerInfo': { ar: 'معلومات العميل', en: 'Customer Information' },
        'orders.addProducts': { ar: 'إضافة المنتجات', en: 'Add Products' },
        'orders.orderedProducts': { ar: 'المنتجات المطلوبة', en: 'Ordered Products' },
        'orders.searchProduct': { ar: 'البحث عن منتج', en: 'Search for a product' },
        'orders.qty': { ar: 'الكمية', en: 'Quantity' },
        'orders.unit': { ar: 'الوحدة', en: 'Unit' },
        'orders.addItem': { ar: 'إضافة صنف', en: 'Add Item' },
        'orders.emptyTitle': { ar: 'لم تتم إضافة أي منتجات بعد', en: 'No products added yet' },
        'orders.emptyDesc': { ar: 'ابحث عن منتج أعلاه وأضفه إلى طلبك', en: 'Search for a product above and add it to your order' },
        'orders.totalLabel': { ar: 'الإجمالي الكلي', en: 'Grand Total' },
        'orders.submit': { ar: 'إرسال الطلب', en: 'Submit Order' },
        'orders.noticeTip': { ar: 'ملاحظة هامة', en: 'Important Notice' },
        'orders.noticeText': { ar: 'سيتم التواصل معكم خلال 24 ساعة لتأكيد الطلب والاتفاق على التفاصيل النهائية والأسعار. الطلبيات الكبيرة تحتاج إلى حجز مسبق بـ 3-5 أيام على الأقل.', en: 'We will contact you within 24 hours to confirm the order and agree on final details and prices. Large orders require a pre-booking of at least 3–5 days.' },
        'orders.colProduct': { ar: 'المنتج', en: 'Product' },
        'orders.colQty': { ar: 'الكمية', en: 'Qty' },
        'orders.colUnit': { ar: 'الوحدة', en: 'Unit' },
        'orders.colPrice': { ar: 'السعر', en: 'Price' },
        'orders.colTotal': { ar: 'المجموع', en: 'Total' },
        'orders.colDelete': { ar: 'حذف', en: 'Delete' },

        /* ── Checkout Page ── */
        'checkout.title': { ar: 'إتمام الطلب', en: 'Checkout' },
        'checkout.subtitle': { ar: 'الرجاء إدخال بياناتك لإيصال طلبك بأمان', en: 'Please enter your details to deliver your order safely' },
        'checkout.firstName': { ar: 'الاسم الأول', en: 'First Name' },
        'checkout.lastName': { ar: 'الاسم الأخير', en: 'Last Name' },
        'checkout.phone': { ar: 'رقم الهاتف', en: 'Phone Number' },
        'checkout.email': { ar: 'البريد الإلكتروني', en: 'Email' },
        'checkout.address': { ar: 'العنوان التفصيلي', en: 'Detailed Address' },
        'checkout.city': { ar: 'المدينة', en: 'City' },
        'checkout.country': { ar: 'الدولة', en: 'Country' },
        'checkout.zip': { ar: 'الرمز البريدي', en: 'Zip Code' },
        'checkout.submit': { ar: 'تأكيد الطلب', en: 'Confirm Order' },

        /* ── Footer ── */
        'footer.brand.desc': { ar: 'ارتق بلحظاتك مع حلويات الاسمر أفضل محل حلويات في الشرق الأوسط. نقدم لكم أجود أنواع الحلويات العربية والغربية المحضرة بحب واتقان.', en: 'Elevate your moments with Al-Asmar Sweets — the best sweet shop in the Middle East. We offer the finest Arabic and Western sweets prepared with love and craftsmanship.' },
        'footer.aboutUs': { ar: 'نبذة عنا', en: 'About Us' },
        'footer.story': { ar: 'قصتنا', en: 'Our Story' },
        'footer.vision': { ar: 'رؤيتنا', en: 'Our Vision' },
        'footer.mission': { ar: 'مهمتنا', en: 'Our Mission' },
        'footer.factory': { ar: 'المصنع', en: 'Factory' },
        'footer.articles': { ar: 'المقالات', en: 'Articles' },
        'footer.importantLinks': { ar: 'روابط مهمة', en: 'Important Links' },
        'footer.privacy': { ar: 'سياسة الخصوصية', en: 'Privacy Policy' },
        'footer.terms': { ar: 'الشروط والأحكام', en: 'Terms & Conditions' },
        'footer.shipping': { ar: 'سياسة الشحن', en: 'Shipping Policy' },
        'footer.awards': { ar: 'الجوائز', en: 'Awards' },
        'footer.contact': { ar: 'ابقى على تواصل', en: 'Stay in Touch' },
        'footer.rights': { ar: 'جميع الحقوق محفوظة.', en: 'All rights reserved.' },

        /* ── Dynamic Layout Elements ── */
        'nav.all': { ar: 'جميع الأصناف', en: 'All Categories' },
        'nav.browseCategories': { ar: 'تصفح الأقسام', en: 'Browse Categories' },
        'nav.arabic': { ar: 'حلويات عربية', en: 'Arabic Sweets' },
        'nav.kunafa': { ar: 'كنافة', en: 'Kunafa' },
        'nav.western': { ar: 'الحلويات الغربية', en: 'Western Sweets' },
        'nav.chocolate': { ar: 'الشوكلاتة والهدايا', en: 'Chocolate & Gifts' },
        'nav.special': { ar: 'عبوات خاصة', en: 'Special Packs' },
        'nav.ice': { ar: 'بوظة', en: 'Ice Cream' },
        'nav.orders': { ar: 'طلبيات', en: 'Orders' },
        'nav.branches': { ar: 'فروعنا', en: 'Branches' },
        'nav.admin': { ar: 'لوحة التحكم', en: 'Dashboard' },

        /* ── Product List Page ── */
        'list.prodCountText': { ar: 'منتج متاح في هذا القسم', en: 'Products available in this category' },
        'home.browseTitle': { ar: 'تصفح جميع الأقسام', en: 'Browse All Categories' },
        'home.browseSub': { ar: 'اكتشف تشكيلتنا الواسعة من الحلويات والهدايا', en: 'Discover our wide selection of sweets and gifts' },

        /* ── Dynamic Category Exact Matches ── */
        'حلويات عربية': { ar: 'حلويات عربية', en: 'Arabic Sweets' },
        'كنافة': { ar: 'كنافة', en: 'Kunafa' },
        'الحلويات الغربية': { ar: 'الحلويات الغربية', en: 'Western Sweets' },
        'الشوكلاتة والهدايا والمناسبات الخاصة': { ar: 'الشوكلاتة والهدايا والمناسبات الخاصة', en: 'Chocolate, Gifts & Special Occasions' },
        'عبوات خاصة': { ar: 'عبوات خاصة', en: 'Special Packages' },
        'البوظة': { ar: 'البوظة', en: 'Ice Cream' },

        /* ── Dynamic Product Exact Matches (Name) ── */
        'بقلاوة بالفستق': { ar: 'بقلاوة بالفستق', en: 'Pistachio Baklava' },
        'بقلاوة بالجوز': { ar: 'بقلاوة بالجوز', en: 'Walnut Baklava' },
        'معمول بالتمر': { ar: 'معمول بالتمر', en: 'Date Maamoul' },
        'معمول بالجوز': { ar: 'معمول بالجوز', en: 'Walnut Maamoul' },
        'Baklava': { ar: 'بقلاوة', en: 'Baklava' },
        'كنافة ناعمة': { ar: 'كنافة ناعمة', en: 'Soft Kunafa' },
        'كنافة خشنة': { ar: 'كنافة خشنة', en: 'Rough Kunafa' },
        'كنافة مكس': { ar: 'كنافة مكس', en: 'Mixed Kunafa' },
        'كنافة وبوظة': { ar: 'كنافة وبوظة', en: 'Kunafa with Ice Cream' },
        'وربات كنافة': { ar: 'وربات كنافة', en: 'Kunafa Warbat' },
        'فطاير مثلثه': { ar: 'فطاير مثلثه', en: 'Triangle Pies' },
        'مخدات': { ar: 'مخدات', en: 'Pillows (Pastries)' },
        'ليزي كيك': { ar: 'ليزي كيك', en: 'Lazy Cake' },
        'مولتن كيك': { ar: 'مولتن كيك', en: 'Molten Cake' },
        'تيراميسو': { ar: 'تيراميسو', en: 'Tiramisu' },
        'ميل فوي': { ar: 'ميل فوي', en: 'Mille-feuille' },
        'Chocolate Cake': { ar: 'كيكة الشوكولاتة', en: 'Chocolate Cake' },
        'Strawberry Cheesecake': { ar: 'تشيز كيك بالفراولة', en: 'Strawberry Cheesecake' },
        'Red Velvet Cake': { ar: 'كيكة ريد فيلفيت', en: 'Red Velvet Cake' },
        'Carrot Cake': { ar: 'كيكة الجزر', en: 'Carrot Cake' },
        'Cookies and Cream Cake': { ar: 'كيكة الكوكيز والكريم', en: 'Cookies and Cream Cake' },
        'Tiramisu': { ar: 'تيراميسو', en: 'Tiramisu' },
        'Glazed Donut': { ar: 'دونات جليزد', en: 'Glazed Donut' },
        'Croissant': { ar: 'كرواسون', en: 'Croissant' },
        'Brownies': { ar: 'براونيز', en: 'Brownies' },
        'Chocolate Chip Cookies': { ar: 'كوكيز رقائق الشوكولاتة', en: 'Chocolate Chip Cookies' },
        'Peanut Butter Cookies': { ar: 'كوكيز زبدة الفول السوداني', en: 'Peanut Butter Cookies' },
        'Oatmeal Raisin Cookies': { ar: 'كوكيز الشوفان والزبيب', en: 'Oatmeal Raisin Cookies' },
        'Blueberry Muffin': { ar: 'مافن التوت الأزرق', en: 'Blueberry Muffin' },
        'Cinnamon Rolls': { ar: 'لفائف القرفة', en: 'Cinnamon Rolls' },
        'Chocolate Truffles': { ar: 'ترافل الشوكولاتة', en: 'Chocolate Truffles' },
        'Dark Chocolate Bar': { ar: 'لوح شوكولاتة داكنة', en: 'Dark Chocolate Bar' },
        'Gummy Bears': { ar: 'حلوى الدببة', en: 'Gummy Bears' },
        'Lollipops': { ar: 'مصاصات', en: 'Lollipops' },
        'Cotton Candy': { ar: 'غزل البنات', en: 'Cotton Candy' },
        'Caramel Fudge': { ar: 'فدج الكراميل', en: 'Caramel Fudge' },
        'Salted Caramels': { ar: 'كراميل مملح', en: 'Salted Caramels' },
        'صندوق الجواهر الشرقية': { ar: 'صندوق الجواهر الشرقية', en: 'Oriental Jewels Box' },
        'باقة الفستق العاشق': { ar: 'باقة الفستق العاشق', en: 'Loving Pistachio Bouquet' },
        'صينية السلطان للضيافة': { ar: 'صينية السلطان للضيافة', en: 'Sultan Hospitality Tray' },
        'تشكيلة التراث الشامي': { ar: 'تشكيلة التراث الشامي', en: 'Levantine Heritage Assortment' },
        'Macaron': { ar: 'ماكارون', en: 'Macaron' },
        'Berry Tart': { ar: 'تارت التوت', en: 'Berry Tart' },
        'Lemon Tart': { ar: 'تارت الليمون', en: 'Lemon Tart' },
        'Vanilla Ice Cream': { ar: 'آيس كريم فانيليا', en: 'Vanilla Ice Cream' },
        'Mint Chocolate Ice Cream': { ar: 'آيس كريم نعناع وشوكولاتة', en: 'Mint Chocolate Ice Cream' },
        'Pistachio Ice Cream': { ar: 'آيس كريم فستق', en: 'Pistachio Ice Cream' },

        /* ── Dynamic Product Exact Matches (Descriptions) ── */
        'desc.بقلاوة بالفستق': { ar: 'بقلاوة طازجة محشوة بالفستق الحلبي', en: 'Fresh baklava stuffed with Aleppo pistachios' },
        'desc.بقلاوة بالجوز': { ar: 'بقلاوة مقرمشة محشوة بالجوز', en: 'Crispy baklava stuffed with walnuts' },
        'desc.معمول بالتمر': { ar: 'معمول طازج محشو بالتمر الفاخر', en: 'Fresh maamoul stuffed with premium dates' },
        'desc.معمول بالجوز': { ar: 'معمول طازج محشو بالجوز المحمص', en: 'Fresh maamoul stuffed with roasted walnuts' },
        'desc.Baklava': { ar: 'Traditional Middle Eastern pastry with honey and pistachios', en: 'Traditional Middle Eastern pastry with honey and pistachios' },
        'desc.كنافة ناعمة': { ar: 'كنافة ناعمة طازجة بالجبنة والقطر', en: 'Soft fresh kunafa with cheese and syrup' },
        'desc.كنافة خشنة': { ar: 'كنافة خشنة مقرمشة بالجبنة الطازجة', en: 'Rough crispy kunafa with fresh cheese' },
        'desc.كنافة مكس': { ar: 'مزيج لذيذ من الكنافة الناعمة والخشنة', en: 'Delicious mix of soft and rough kunafa' },
        'desc.كنافة وبوظة': { ar: 'كنافة ساخنة مع بوظة باردة', en: 'Hot kunafa with cold ice cream' },
        'desc.وربات كنافة': { ar: 'وربات محشوة بالقشطة أو الجبنة', en: 'Warbat stuffed with cream or cheese' },
        'desc.فطاير مثلثه': { ar: 'فطاير مثلثة محشوة بالجبنة', en: 'Triangle pies stuffed with cheese' },
        'desc.مخدات': { ar: 'معجنات على شكل مخدات محشوة بالجبنة', en: 'Pillow-shaped pastries stuffed with cheese' },
        'desc.ليزي كيك': { ar: 'كيكة البسكويت بالشوكولاتة اللذيذة', en: 'Delicious chocolate biscuit cake' },
        'desc.مولتن كيك': { ar: 'كيكة الشوكولاتة الدافئة بقلب غني يذوب عند أول قضمة', en: 'Warm chocolate cake with a rich melting heart' },
        'desc.تيراميسو': { ar: 'الحلوى الإيطالية الفاخرة بطبقات البسكويت المشبعة بالقهوة', en: 'Luxurious Italian dessert with coffee-soaked biscuit layers' },
        'desc.ميل فوي': { ar: 'رقائق الهشاشة الفرنسية التقليدية مع طبقات الكاسترد', en: 'Traditional French flaky pastry with custard layers' },
        'desc.Chocolate Cake': { ar: 'كيكة الشوكولاتة الغنية مع كريمة الفدج', en: 'Rich chocolate cake with fudge frosting' },
        'desc.Strawberry Cheesecake': { ar: 'تشيز كيك كلاسيكي مع الفراولة الطازجة', en: 'Classic cheesecake with fresh strawberries' },
        'desc.Red Velvet Cake': { ar: 'كيكة ريد فيلفيت رطبة مع كريمة الجبن', en: 'Moist red velvet cake with cream cheese frosting' },
        'desc.Carrot Cake': { ar: 'كيكة الجزر الرطبة مع كريمة الجبن والجوز', en: 'Moist carrot cake with cream cheese frosting and walnuts' },
        'desc.Cookies and Cream Cake': { ar: 'طبقات من كيك الشوكولاتة مع حشوة الأوريو', en: 'Chocolate cake layers with Oreo filling' },
        'desc.Tiramisu': { ar: 'الحلوى الإيطالية الكلاسيكية', en: 'Classic Italian dessert' },
        'desc.Glazed Donut': { ar: 'دونات كلاسيكي طازج مع طبقة سكر لامعة', en: 'Fresh classic donut with a glossy sugar glaze' },
        'desc.Croissant': { ar: 'كرواسون فرنسي مخبوز بالزبدة', en: 'Butter-baked French croissant' },
        'desc.Brownies': { ar: 'براونيز الشوكولاتة الغنية مع الجوز', en: 'Rich chocolate brownies with walnuts' },
        'desc.Chocolate Chip Cookies': { ar: 'كوكيز الشوكولاتة المنزلي', en: 'Homemade chocolate chip cookies' },
        'desc.Peanut Butter Cookies': { ar: 'كوكيز زبدة الفول السوداني الكلاسيكي', en: 'Classic peanut butter cookies' },
        'desc.Oatmeal Raisin Cookies': { ar: 'كوكيز الشوفان بالزبيب', en: 'Oatmeal raisin cookies' },
        'desc.Blueberry Muffin': { ar: 'مافن طازج محشو بالتوت الأزرق', en: 'Fresh muffin stuffed with blueberries' },
        'desc.Cinnamon Rolls': { ar: 'رول القرفة الدافئ مع كريمة الجبن', en: 'Warm cinnamon roll with cream cheese frosting' },
        'desc.Chocolate Truffles': { ar: 'ترافل الشوكولاتة المصنوع يدوياً', en: 'Handcrafted chocolate truffles' },
        'desc.Dark Chocolate Bar': { ar: 'لوح شوكولاتة داكنة 70%', en: '70% dark chocolate bar' },
        'desc.Gummy Bears': { ar: 'سكاكر جيلي بنكهات الفواكه المتنوعة', en: 'Jelly candies in assorted fruit flavors' },
        'desc.Lollipops': { ar: 'مصاصات ملونة بنكهات الفواكه', en: 'Colorful fruit-flavored lollipops' },
        'desc.Cotton Candy': { ar: 'غزل البنات الخفيف بنكهات متنوعة', en: 'Light cotton candy in assorted flavors' },
        'desc.Caramel Fudge': { ar: 'فدج الكراميل الناعم والكريمي', en: 'Smooth and creamy caramel fudge' },
        'desc.Salted Caramels': { ar: 'كراميل فاخر بالملح المتوازن', en: 'Premium caramels with balanced sea salt' },
        'desc.صندوق الجواهر الشرقية': { ar: 'تشكيلة فاخرة من أرقى الحلويات العربية', en: 'Luxurious assortment of the finest Arabic sweets' },
        'desc.باقة الفستق العاشق': { ar: 'حلويات عربية فاخرة محشوة بالفستق الحلبي', en: 'Luxurious Arabic sweets stuffed with Aleppo pistachios' },
        'desc.صينية السلطان للضيافة': { ar: 'مجموعة مميزة من الحلويات العربية للمناسبات', en: 'Distinctive collection of Arabic sweets for occasions' },
        'desc.تشكيلة التراث الشامي': { ar: 'أرقى أنواع الحلويات الشامية التقليدية', en: 'The finest traditional Levantine sweets' },
        'desc.Macaron': { ar: 'ماكارون فرنسي أنيق بنكهات متنوعة', en: 'Elegant French macarons in assorted flavors' },
        'desc.Berry Tart': { ar: 'تارت هش محشو بالكاسترد ومغطى بالتوت المشكل', en: 'Crispy tart filled with custard and topped with mixed berries' },
        'desc.Lemon Tart': { ar: 'تارت الليمون اللاذع مع قشرة مقرمشة', en: 'Tangy lemon tart with a crispy crust' },
        'desc.Vanilla Ice Cream': { ar: 'بوظة فانيليا كريمية مع حبوب الفانيليا الطبيعية', en: 'Creamy vanilla ice cream with natural vanilla beans' },
        'desc.Mint Chocolate Ice Cream': { ar: 'بوظة النعناع المنعشة مع رقائق الشوكولاتة', en: 'Refreshing mint ice cream with chocolate chips' },
        'desc.Pistachio Ice Cream': { ar: 'بوظة الفستق الغنية المصنوعة من المكسرات الطبيعية', en: 'Rich pistachio ice cream made with natural nuts' },

        /* ── Dynamic Category Exact Matches ── */
        'بوظة': { ar: 'بوظة', en: 'Ice Cream' },
        'الشوكولاتة والهدايا': { ar: 'الشوكولاتة والهدايا', en: 'Chocolate & Gifts' },

        /* ── Orders Page form labels ── */
        'orders.custNameLabel': { ar: 'اسم العميل', en: 'Customer Name' },
        'orders.phoneLabel': { ar: 'رقم الهاتف', en: 'Phone Number' },
        'orders.dateLabel': { ar: 'تاريخ المناسبة', en: 'Occasion Date' },
        'orders.notesLabel': { ar: 'ملاحظات إضافية', en: 'Additional Notes' },
        'orders.namePh': { ar: 'أدخل اسمك الكامل', en: 'Enter your full name' },
        'orders.notesPh': { ar: 'أي ملاحظات أو طلبات خاصة...', en: 'Any notes or special requests...' },
        'orders.unit.kilo': { ar: 'كيلو', en: 'Kilo' },
        'orders.unit.tray': { ar: 'صينية', en: 'Tray' },
        'orders.unit.piece': { ar: 'قطعة', en: 'Piece' },
        'orders.unit.item': { ar: 'حبة', en: 'Item' },
        'orders.unit.pack': { ar: 'علبة', en: 'Pack' },

        /* ── Login Page extra labels ── */
        'login.email': { ar: 'البريد الإلكتروني', en: 'Email' },
        'login.password': { ar: 'كلمة المرور', en: 'Password' },
        'login.rememberMe': { ar: 'تذكرني؟', en: 'Remember me?' },
        'login.benefitsText': { ar: 'بإنشاء حساب معنا، ستتمكن من:', en: 'By creating an account with us, you will be able to:' },

        /* ── Branches dynamic data ── */
        'فرع عمان - الدوار السابع': { ar: 'فرع عمان - الدوار السابع', en: 'Amman - 7th Circle Branch' },
        'عمان، الدوار السابع، شارع عبدالله غوشة، مجمع السابع التجاري': { ar: 'عمان، الدوار السابع، شارع عبدالله غوشة، مجمع السابع التجاري', en: 'Amman, 7th Circle, Abdullah Ghosheh St, 7th Commercial Complex' },
        '9:00 صباحاً - 11:00 مساءً': { ar: '9:00 صباحاً - 11:00 مساءً', en: '9:00 AM - 11:00 PM' },
        'فرع الزرقاء - السوق الجديد': { ar: 'فرع الزرقاء - السوق الجديد', en: 'Zarqa - New Market Branch' },
        'الزرقاء، السوق الجديد، شارع الملك عبدالله الثاني، بجانب البنك العربي': { ar: 'الزرقاء، السوق الجديد، شارع الملك عبدالله الثاني، بجانب البنك العربي', en: 'Zarqa, New Market, King Abdullah II St, next to Arab Bank' },
        '8:30 صباحاً - 10:30 مساءً': { ar: '8:30 صباحاً - 10:30 مساءً', en: '8:30 AM - 10:30 PM' },
        'فرع إربد - شارع الجامعة': { ar: 'فرع إربد - شارع الجامعة', en: 'Irbid - University St Branch' },
        'إربد، شارع الجامعة، مقابل بوابة جامعة اليرموك الشمالية': { ar: 'إربد، شارع الجامعة، مقابل بوابة جامعة اليرموك الشمالية', en: 'Irbid, University St, opp. Yarmouk Uni North Gate' },
        '9:00 صباحاً - 11:30 مساءً': { ar: '9:00 صباحاً - 11:30 مساءً', en: '9:00 AM - 11:30 PM' },

        /* ── Info Pages ── */

        /* Story */
        'info.story.title': { ar: 'قصة حلويات الأسمر', en: 'The Story of Al-Asmar Sweets' },
        'info.story.subtitle': { ar: 'أكثر من مجرد حلويات، إنها قصة عشق للأصالة', en: 'More than just sweets, it\'s a love story for authenticity' },
        'info.story.p1': { ar: 'بدأت رحلتنا في قلب مدينة عمان، برغبة بسيطة ولكنها عميقة: تقديم حلويات شرقية لا تكتفي بأن تكون لذيذة، بل تحكي قصة التراث العريق في كل قضمة.', en: 'Our journey began in the heart of Amman, with a simple yet profound desire: to offer oriental sweets that are not only delicious but also tell the story of deep-rooted heritage in every bite.' },
        'info.story.p2': { ar: 'تأسست حلويات الأسمر بشغف لا يضاهى لصناعة الكنافة والحلويات العربية الأصيلة. من مطبخ صغير وعائلة محبة، كبرنا لنصبح وجهة لكل عشاق المذاق الرفيع، حافظنا خلالها على أسرار الوصفات المتوارثة، واستخدمنا أجود أنواع السمن البلدي والفستق الحلبي، لنضمن لكم تجربة لا تنسى.', en: 'Al-Asmar Sweets was founded with unparalleled passion for crafting authentic Kunafa and Arabic sweets. From a small kitchen and a loving family, we grew into a destination for all lovers of refined taste, preserving inherited recipe secrets and using the finest ghee and Aleppo pistachios, to ensure an unforgettable experience.' },
        'info.story.p3': { ar: 'في حلويات الأسمر، نحن لا نصنع الحلوى فحسب، بل نصنع الذكريات التي تجمع العائلة والأحباب.', en: 'At Al-Asmar Sweets, we don\'t just make sweets, we make memories that bring family and loved ones together.' },
        'info.story.startTitle': { ar: 'البداية', en: 'The Beginning' },
        'info.story.startDesc': { ar: 'انطلقت مسيرتنا من شغف صغير وحلم كبير بتقديم الأفضل.', en: 'Our journey started from a small passion and a big dream to offer the best.' },
        'info.story.familyTitle': { ar: 'العائلة', en: 'The Family' },
        'info.story.familyDesc': { ar: 'نعتبر زبائننا جزءاً من عائلتنا الكبيرة، ونسعى دائماً لإرضائهم.', en: 'We consider our customers part of our big family, and always strive to satisfy them.' },
        'info.story.excellenceTitle': { ar: 'التميز', en: 'Excellence' },
        'info.story.excellenceDesc': { ar: 'نلتزم بأعلى معايير الجودة والنظافة في كل ما نقدمه.', en: 'We are committed to the highest standards of quality and hygiene in everything we offer.' },

        /* Terms */
        'info.terms.title': { ar: 'الشروط والأحكام', en: 'Terms and Conditions' },
        'info.terms.subtitle': { ar: 'يرجى قراءة هذه الشروط بعناية قبل استخدام موقعنا.', en: 'Please read these terms carefully before using our website.' },
        'info.terms.sec1Title': { ar: '1. قبول الشروط', en: '1. Acceptance of Terms' },
        'info.terms.sec1Desc': { ar: 'بوصولك واستخدامك لموقع حلويات الأسمر، فإنك توافق على الالتزام بهذه الشروط والأحكام وجميع القوانين واللوائح المعمول بها.', en: 'By accessing and using the Al-Asmar Sweets website, you agree to be bound by these terms and conditions and all applicable laws and regulations.' },
        'info.terms.sec2Title': { ar: '2. الطلبات والدفع', en: '2. Orders and Payment' },
        'info.terms.sec2Desc': { ar: 'يجب أن تكون جميع المعلومات المقدمة أثناء عملية الطلب دقيقة وكاملة. نحتفظ بالحق في رفض أو إلغاء أي طلب لأي سبب من الأسباب. الدفع يتم بالوسائل المتاحة والمعتمدة على الموقع.', en: 'All information provided during the ordering process must be accurate and complete. We reserve the right to refuse or cancel any order for any reason. Payment is made through the available and approved methods on the site.' },
        'info.terms.sec3Title': { ar: '3. سياسة الاسترجاع', en: '3. Return Policy' },
        'info.terms.sec3Desc': { ar: 'نظراً لطبيعة منتجاتنا الغذائية، لا يمكن استرجاع المنتجات بعد استلامها إلا في حال وجود خطأ في الطلب أو مشكلة في الجودة. يرجى التواصل معنا خلال 24 ساعة من الاستلام في حال وجود أي مشكلة.', en: 'Due to the nature of our food products, items cannot be returned after receipt unless there is an error in the order or a quality issue. Please contact us within 24 hours of receipt if there is any problem.' },
        'info.terms.sec4Title': { ar: '4. حقوق الملكية الفكرية', en: '4. Intellectual Property Rights' },
        'info.terms.sec4Desc': { ar: 'جميع المحتويات الموجودة على هذا الموقع، بما في ذلك النصوص والتصاميم والشعارات والصور، هي ملكية حصرية لحلويات الأسمر ومحمية بموجب قوانين حقوق النشر.', en: 'All content on this site, including text, designs, logos, and images, is the exclusive property of Al-Asmar Sweets and is protected by copyright laws.' },

        /* Awards */
        'info.awards.title': { ar: 'جوائزنا وإنجازاتنا', en: 'Our Awards and Achievements' },
        'info.awards.subtitle': { ar: 'فخرنا لا يكمن في الجوائز، بل في ثقتكم التي منحتونا إياها', en: 'Our pride lies not in awards, but in the trust you have given us' },
        'info.awards.a1Title': { ar: 'أفضل محل حلويات 2025', en: 'Best Sweet Shop 2025' },
        'info.awards.a1Desc': { ar: 'جائزة التميز في صناعة الحلويات العربية - مهرجان المذاق العربي.', en: 'Award of Excellence in the Arabic Sweets Industry - Arab Taste Festival.' },
        'info.awards.a2Title': { ar: 'شهادة الآيزو 22000', en: 'ISO 22000 Certification' },
        'info.awards.a2Desc': { ar: 'شهادة عالمية في إدارة سلامة الغذاء وجودة التصنيع.', en: 'Global certification in food safety management and manufacturing quality.' },
        'info.awards.a3Title': { ar: 'درع الخدمة المجتمعية', en: 'Community Service Shield' },
        'info.awards.a3Desc': { ar: 'تكريم لجهودنا في دعم الأنشطة الخيرية والمجتمعية.', en: 'Recognition of our efforts in supporting charitable and community activities.' },

        /* Articles */
        'info.articles.title': { ar: 'مدونة الحلويات', en: 'Sweets Blog' },
        'info.articles.subtitle': { ar: 'اكتشف أسرار الحلويات، نصائح التقديم، وقصص من التراث', en: 'Discover sweets secrets, presentation tips, and stories from heritage' },
        'info.articles.badgeHeritage': { ar: 'تراث', en: 'Heritage' },
        'info.articles.art1Title': { ar: 'تاريخ الكنافة: رحلة عبر الزمن', en: 'The History of Kunafa: A Journey Through Time' },
        'info.articles.art1Desc': { ar: 'هل تعلم أن الكنافة تعود جذورها إلى العصر الأموي؟ اكتشف القصة الكاملة لهذا الطبق الشهي وكيف تطور عبر العصور.', en: 'Did you know that Kunafa traces its roots back to the Umayyad era? Discover the full story of this delicious dish and how it evolved through the ages.' },
        'info.articles.readMore': { ar: 'اقرأ المزيد', en: 'Read More' },
        'info.articles.badgeTips': { ar: 'نصائح', en: 'Tips' },
        'info.articles.art2Title': { ar: 'فن تقديم القهوة مع الحلويات الشرقية', en: 'The Art of Serving Coffee with Oriental Sweets' },
        'info.articles.art2Desc': { ar: 'تعرف على أفضل أنواع القهوة التي تبرز نكهة البقلاوة والمعمول، وكيفية تنسيق مائدة ضيافة تبهر ضيوفك.', en: 'Learn about the best coffees that highlight the flavors of Baklava and Maamoul, and how to arrange a hospitality table that dazzles your guests.' },
        'info.articles.badgeHealth': { ar: 'صحة', en: 'Health' },
        'info.articles.art3Title': { ar: 'هل يمكن أن تكون الحلويات صحية؟', en: 'Can Sweets Be Healthy?' },
        'info.articles.art3Desc': { ar: 'اكتشف خياراتنا الجديدة من الحلويات قليلة السكر والمحضرة بمكونات طبيعية، للاستمتاع بالطعم دون تأنيب الضمير.', en: 'Discover our new options of low-sugar sweets prepared with natural ingredients, to enjoy the taste without any guilt.' },

        /* Factory */
        'info.factory.title': { ar: 'جولة في مصنعنا', en: 'A Tour of Our Factory' },
        'info.factory.subtitle': { ar: 'حيث يمتزج الفن بالدقة لصناعة أشهى الحلويات', en: 'Where art blends with precision to create the most delicious sweets' },
        'info.factory.sec1Title': { ar: 'تكنولوجيا حديثة بلمسة تقليدية', en: 'Modern Technology with a Traditional Touch' },
        'info.factory.sec1P1': { ar: 'يمتد مصنع حلويات الأسمر على مساحة واسعة، مجهز بأحدث الآلات وخطوط الإنتاج العالمية التي تضمن الدقة والنظافة في كل خطوة. ومع ذلك، نحن نؤمن بأن الآلة لا تغني عن اللمسة البشرية الماهرة.', en: 'The Al-Asmar Sweets factory spans a vast area, equipped with the latest machinery and global production lines that ensure precision and hygiene at every step. However, we believe that machinery cannot replace the skilled human touch.' },
        'info.factory.sec1P2': { ar: 'فريقنا المكون من أمهر الطهاة والحرفيين يشرف على كل تفصيل، من تحضير العجين إلى تزيين القطع النهائية، لنحافظ على النكهة البيتية الأصيلة التي تميزنا.', en: 'Our team, composed of the most skilled chefs and artisans, supervises every detail, from preparing the dough to decorating the final pieces, to maintain the authentic homemade flavor that distinguishes us.' },
        'info.factory.foodSafety': { ar: 'نظام سلامة الغذاء', en: 'Food Safety System' },
        'info.factory.qualityCert': { ar: 'شهادة الجودة', en: 'Quality Certificate' },
        'info.factory.cleanliness': { ar: 'نظافة وتعقيم', en: 'Cleanliness and Sterilization' },
        'info.factory.standardsTitle': { ar: 'معاييرنا في الإنتاج', en: 'Our Production Standards' },
        'info.factory.st1': { ar: 'تعقيم دوري ومستمر لكافة المرافق.', en: 'Periodic and continuous sterilization of all facilities.' },
        'info.factory.st2': { ar: 'فحوصات مخبرية للمواد الخام والمنتج النهائي.', en: 'Laboratory tests for raw materials and final products.' },
        'info.factory.st3': { ar: 'تغليف آمن يحافظ على الطزاجة.', en: 'Secure packaging that preserves freshness.' },
        'info.factory.st4': { ar: 'التزام كامل بمعايير الصحة والسلامة المهنية.', en: 'Total commitment to occupational health and safety standards.' },

        /* Mission */
        'info.mission.title': { ar: 'مهمتنا ورسالتنا', en: 'Our Mission and Message' },
        'info.mission.subtitle': { ar: 'الجودة، الإتقان، والسعادة في كل قضمة', en: 'Quality, mastery, and happiness in every bite' },
        'info.mission.p1': { ar: 'مهمتنا واضحة وبسيطة: رسم الابتسامة على وجوه عملائنا من خلال تقديم حلويات تفوق التوقعات. نحن نؤمن بأن الحلويات ليست مجرد صنف غذائي، بل هي وسيلة للتعبير عن الفرح والمحبة.', en: 'Our mission is clear and simple: putting a smile on our customers\' faces by offering sweets that exceed expectations. We believe that sweets are not just a food item, but a way to express joy and love.' },
        'info.mission.p2': { ar: 'نعمل يومياً بجد وتفانٍ لضمان أعلى مستويات الجودة في كل مرحلة من مراحل التحضير، من اختيار المكونات الطازجة بعناية فائقة، إلى عمليات الخبز المتقنة، وصولاً إلى التقديم الراقي الذي يليق بكم.', en: 'We work hard and with dedication every day to ensure the highest quality standards at every stage of preparation, from carefully selecting fresh ingredients to meticulous baking processes, and ultimately to the elegant presentation you deserve.' },
        'info.mission.commitment': { ar: 'التزامنا:', en: 'Our Commitment:' },
        'info.mission.commitmentDesc': { ar: 'تقديم تجربة تذوق استثنائية تجمع بين التراث الأصيل والإبداع المعاصر.', en: 'Delivering an exceptional tasting experience that combines authentic heritage with contemporary creativity.' },
        'info.mission.highQuality': { ar: 'الجودة العالية', en: 'High Quality' },
        'info.mission.highQualityDesc': { ar: 'استخدام أفضل المكونات الطبيعية 100% بدون أي مواد حافظة.', en: 'Using the best 100% natural ingredients without any preservatives.' },
        'info.mission.heartService': { ar: 'خدمة من القلب', en: 'Heartfelt Service' },
        'info.mission.heartServiceDesc': { ar: 'فريقنا مدرب لخدمتكم بكل ود واحترام لتلبية كافة الرغبات.', en: 'Our team is trained to serve you with friendliness and respect to meet all your desires.' },
        'info.mission.socialResp': { ar: 'مسؤوليتنا الاجتماعية', en: 'Our Social Responsibility' },
        'info.mission.socialRespDesc': { ar: 'ندعم مجتمعنا ونساهم في المبادرات الخيرية كجزء من واجبنا.', en: 'We support our community and contribute to charitable initiatives as part of our duty.' },

        /* Privacy */
        'info.privacy.title': { ar: 'سياسة الخصوصية', en: 'Privacy Policy' },
        'info.privacy.lastUpdated': { ar: 'تاريخ آخر تحديث: 14 فبراير 2026', en: 'Last Updated: February 14, 2026' },
        'info.privacy.introTitle': { ar: '1. مقدمة', en: '1. Introduction' },
        'info.privacy.introDesc': { ar: 'نحن في حلويات الأسمر نلتزم بحماية خصوصيتك ومعلوماتك الشخصية. توضح سياسة الخصوصية هذه كيفية جمع واستخدام وحماية المعلومات التي تقدمها لنا عند استخدام موقعنا الإلكتروني.', en: 'At Al-Asmar Sweets, we are committed to protecting your privacy and personal information. This privacy policy explains how we collect, use, and protect the information you provide us when using our website.' },
        'info.privacy.collectTitle': { ar: '2. المعلومات التي نجمعها', en: '2. Information We Collect' },
        'info.privacy.collect1': { ar: 'الاسم ومعلومات الاتصال (الهاتف، البريد الإلكتروني).', en: 'Name and contact information (Phone, Email).' },
        'info.privacy.collect2': { ar: 'معلومات الشحن والتوصيل.', en: 'Shipping and delivery information.' },
        'info.privacy.collect3': { ar: 'سجل الطلبات والتفضيلات.', en: 'Order history and preferences.' },
        'info.privacy.useTitle': { ar: '3. كيفية استخدام المعلومات', en: '3. How We Use Information' },
        'info.privacy.useDesc': { ar: 'نستخدم معلوماتك للأغراض التالية:', en: 'We use your information for the following purposes:' },
        'info.privacy.use1': { ar: 'لمعالجة طلباتك وتوصيلها.', en: 'To process and deliver your orders.' },
        'info.privacy.use2': { ar: 'لتحسين خدماتنا ومنتجاتنا.', en: 'To improve our services and products.' },
        'info.privacy.use3': { ar: 'للتواصل معك بخصوص العروض والمنتجات الجديدة (بموافقتك).', en: 'To contact you regarding new offers and products (with your consent).' },
        'info.privacy.protectTitle': { ar: '4. حماية المعلومات', en: '4. Information Protection' },
        'info.privacy.protectDesc': { ar: 'نحن نتخذ كافة التدابير الأمنية اللازمة لحماية معلوماتك الشخصية من الوصول غير المصرح به أو التعديل أو الإفشاء.', en: 'We take all necessary security measures to protect your personal information from unauthorized access, alteration, or disclosure.' },
        'info.privacy.contactUs': { ar: 'إذا كان لديك أي استفسارات حول سياسة الخصوصية، يرجى التواصل معنا عبر البريد الإلكتروني:', en: 'If you have any questions about the Privacy Policy, please contact us via email:' },

        /* Shipping */
        'info.shipping.title': { ar: 'التوصيل والشحن', en: 'Delivery & Shipping' },
        'info.shipping.desc': { ar: 'نحن ندرك أنكم تنتظرون حلوياتنا بفارغ الصبر، لذا نعمل جاهدين لتوصيلها لكم بأسرع وقت وبأفضل حال.', en: 'We understand you eagerly await our sweets, so we work hard to deliver them to you as quickly and in the best condition possible.' },
        'info.shipping.fastTitle': { ar: 'توصيل سريع', en: 'Fast Delivery' },
        'info.shipping.fastDesc': { ar: 'توصيل في نفس اليوم للطلبات قبل الساعة 12 ظهراً.', en: 'Same-day delivery for orders before 12:00 PM.' },
        'info.shipping.coldTitle': { ar: 'توصيل مبرد', en: 'Refrigerated Delivery' },
        'info.shipping.coldDesc': { ar: 'سيارات مجهزة للحفاظ على جودة وطراوة الحلويات.', en: 'Equipped vehicles to maintain the quality and freshness of the sweets.' },
        'info.shipping.coverTitle': { ar: 'تغطية واسعة', en: 'Wide Coverage' },
        'info.shipping.coverDesc': { ar: 'نصل إليكم أينما كنتم في جميع أنحاء المملكة.', en: 'We reach you wherever you are across the Kingdom.' },
        'info.shipping.ammanTitle': { ar: 'داخل عمان', en: 'Inside Amman' },
        'info.shipping.ammanDesc': { ar: 'التوصيل خلال 2-4 ساعات', en: 'Delivery within 2-4 hours' },
        'info.shipping.ammanPrice': { ar: '3.00 دينار', en: '3.00 JOD' },
        'info.shipping.otherTitle': { ar: 'المحافظات الأخرى', en: 'Other Governorates' },
        'info.shipping.otherDesc': { ar: 'التوصيل خلال 24 ساعة', en: 'Delivery within 24 hours' },
        'info.shipping.otherPrice': { ar: '5.00 دينار', en: '5.00 JOD' },

        /* Vision */
        'info.vision.title': { ar: 'رؤيتنا المستقبلية', en: 'Our Future Vision' },
        'info.vision.subtitle': { ar: 'نحو العالمية بنكهة عربية أصيلة', en: 'Towards global reach with an authentic Arabic flavor' },
        'info.vision.p1': { ar: 'رؤيتنا في حلويات الأسمر تتجاوز حدود المكان. نحن نطمح لأن نكون السفير الأول للحلويات الشرقية في العالم، حيث ننقل تراثنا الغني ونكهاتنا الفريدة إلى كل مائدة.', en: 'Our vision at Al-Asmar Sweets transcends boundaries. We aspire to be the premier ambassador of oriental sweets globally, conveying our rich heritage and unique flavors to every table.' },
        'info.vision.p2': { ar: 'نرى مستقبلاً حيث ترتبط الفخامة والجودة باسم "الأسمر". نسعى للتوسع والابتكار المستمر مع الحفاظ على روح الأصالة. هدفنا أن نكون الخيار الأول لكل من يبحث عن التميز، وأن تظل حلوياتنا رمزاً للكرم وحسن الضيافة العربية.', en: 'We envision a future where luxury and quality are synonymous with the name "Al-Asmar". We strive for continuous expansion and innovation while preserving the spirit of authenticity. Our goal is to be the first choice for anyone seeking excellence, and for our sweets to remain a symbol of Arab generosity and hospitality.' },
        'info.vision.li1': { ar: 'الريادة في صناعة الحلويات الشرقية', en: 'Pioneering in the oriental sweets industry' },
        'info.vision.li2': { ar: 'الانتشار في أسواق جديدة محلياً وعالمياً', en: 'Expanding into new markets locally and globally' },
        'info.vision.li3': { ar: 'تطوير وصفات مبتكرة تدمج الحاضر بالماضي', en: 'Developing innovative recipes that blend the present with the past' }
    };


    /* ── 2.  Storage key ─────────────────────────────────────────────────── */
    var STORAGE_KEY = 'sweetshop_lang';

    /* ── 3.  Core apply() function ──────────────────────────────────────── */
    function apply(lang) {
        if (lang !== 'ar' && lang !== 'en') lang = 'ar';

        /* a) Update <html> attributes */
        document.documentElement.lang = lang;
        document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';

        /* b) Swap all text nodes */
        var els = document.querySelectorAll('[data-i18n]');
        els.forEach(function (el) {
            var key = el.getAttribute('data-i18n');
            var entry = t[key];
            if (!entry) return;
            // For elements that contain child nodes (like icons), only replace
            // the last text node so Bootstrap Icons remain intact.
            var text = entry[lang] || entry['ar'];
            if (el.childNodes.length > 0) {
                // Find the first plain Text node and replace it;
                // if none exists (only element children) append a Text node.
                var textNodes = Array.from(el.childNodes).filter(function (n) {
                    return n.nodeType === Node.TEXT_NODE;
                });
                if (textNodes.length > 0) {
                    textNodes[0].textContent = text;
                } else {
                    el.insertBefore(document.createTextNode(text), el.firstChild);
                }
            } else {
                el.textContent = text;
            }
        });

        /* c) Swap placeholders */
        var phEls = document.querySelectorAll('[data-i18n-placeholder]');
        phEls.forEach(function (el) {
            var key = el.getAttribute('data-i18n-placeholder');
            var entry = t[key];
            if (entry) el.placeholder = entry[lang] || entry['ar'];
        });

        /* d) Persist preference */
        try { localStorage.setItem(STORAGE_KEY, lang); } catch (e) { /* quota or SSR */ }

        /* e) Update the active marker on the language switcher options */
        document.querySelectorAll('.lang-option').forEach(function (btn) {
            var isActive = btn.getAttribute('data-lang') === lang;
            btn.classList.toggle('lang-option--active', isActive);
        });
    }

    /* ── 4.  Restore on page load ───────────────────────────────────────── */
    function init() {
        var saved = 'ar';
        try { saved = localStorage.getItem(STORAGE_KEY) || 'ar'; } catch (e) { }
        apply(saved);
    }

    /* ── 5.  Public API ─────────────────────────────────────────────────── */
    return {
        apply: apply,
        init: init,
        getSaved: function () {
            try { return localStorage.getItem(STORAGE_KEY) || 'ar'; } catch (e) { return 'ar'; }
        },
        translations: t
    };

}());
