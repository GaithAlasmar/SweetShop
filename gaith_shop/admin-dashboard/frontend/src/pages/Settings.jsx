const Settings = () => {
    return (
        <div dir="rtl">
            <h1 className="text-3xl font-bold text-gray-800 mb-6">إعدادات الموقع</h1>
            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8 max-w-2xl">
                <div className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">اسم الموقع</label>
                        <input type="text" className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary outline-none" defaultValue="متجر حلويات عيد الضحى" />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">رابط الشعار (Logo URL)</label>
                        <input type="text" className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary outline-none" placeholder="https://..." />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">روابط السوشيال ميديا</label>
                        <div className="space-y-2">
                            <input type="text" className="w-full px-4 py-2 border rounded-lg outline-none" placeholder="Facebook" />
                            <input type="text" className="w-full px-4 py-2 border rounded-lg outline-none" placeholder="Instagram" />
                        </div>
                    </div>
                    <button className="bg-primary text-white px-8 py-3 rounded-xl font-bold hover:bg-primary/90 transition-colors">
                        حفظ التغييرات
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Settings;
