import { Link, useLocation } from 'react-router-dom';
import { Home, Users, ShoppingBag, Settings, LogOut, ChevronRight } from 'lucide-react';

const Sidebar = () => {
    const location = useLocation();

    const menuItems = [
        { title: 'الرئيسية', path: '/admin', icon: Home },
        { title: 'إدارة المستخدمين', path: '/admin/users', icon: Users },
        { title: 'إدارة المحتوى', path: '/admin/products', icon: ShoppingBag },
        { title: 'الإعدادات', path: '/admin/settings', icon: Settings },
    ];

    return (
        <div className="w-64 bg-white shadow-xl h-screen flex flex-col border-l border-gray-100 sticky top-0" dir="rtl">
            <div className="p-8 border-b border-gray-50">
                <h1 className="text-2xl font-bold text-primary flex items-center gap-2">
                    متجر <span className="text-accent underline decoration-accent/30">الحلويات</span>
                </h1>
                <p className="text-xs text-gray-400 mt-1 uppercase tracking-widest font-semibold">لوحة التحكم</p>
            </div>

            <nav className="flex-1 px-4 py-6 space-y-2">
                {menuItems.map((item) => {
                    const Icon = item.icon;
                    const isActive = location.pathname === item.path;
                    return (
                        <Link
                            key={item.path}
                            to={item.path}
                            className={`flex items-center justify-between px-4 py-3 rounded-xl transition-all duration-300 group ${isActive
                                    ? 'bg-primary text-white shadow-lg shadow-primary/20 scale-105'
                                    : 'text-gray-500 hover:bg-bgLight hover:text-primary'
                                }`}
                        >
                            <div className="flex items-center gap-3">
                                <Icon size={20} className={isActive ? 'text-white' : 'text-gray-400 group-hover:text-primary'} />
                                <span className="font-medium">{item.title}</span>
                            </div>
                            {isActive && <ChevronRight size={16} />}
                        </Link>
                    );
                })}
            </nav>

            <div className="p-4 border-t border-gray-50">
                <button className="flex items-center gap-3 w-full px-4 py-3 text-red-500 hover:bg-red-50 rounded-xl transition-colors font-medium">
                    <LogOut size={20} />
                    <span>تسجيل الخروج</span>
                </button>
            </div>
        </div>
    );
};

export default Sidebar;
