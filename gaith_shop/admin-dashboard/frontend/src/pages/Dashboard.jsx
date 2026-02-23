import { Users, ShoppingBag, TrendingUp, AlertCircle } from 'lucide-react';

const Dashboard = () => {
    const stats = [
        { title: 'إجمالي المستخدمين', value: '1,284', color: 'bg-blue-500', icon: Users, change: '+12%' },
        { title: 'إجمالي الطلبات', value: '456', color: 'bg-green-500', icon: ShoppingBag, change: '+8%' },
        { title: 'المبيعات اليومية', value: '$2,450', color: 'bg-purple-500', icon: TrendingUp, change: '+15%' },
        { title: 'طلبات معلقة', value: '12', color: 'bg-amber-500', icon: AlertCircle, change: '-4%' },
    ];

    return (
        <div dir="rtl">
            <header className="mb-10">
                <h1 className="text-3xl font-bold text-gray-800">مرحباً بك، أيمن! 👋</h1>
                <p className="text-gray-500 mt-2">إليك نظرة سريعة على أداء متجرك اليوم.</p>
            </header>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
                {stats.map((stat, idx) => {
                    const Icon = stat.icon;
                    return (
                        <div key={idx} className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
                            <div className="flex items-center justify-between mb-4">
                                <div className={`${stat.color} p-3 rounded-xl text-white shadow-lg`}>
                                    <Icon size={24} />
                                </div>
                                <span className={`text-sm font-bold ${stat.change.startsWith('+') ? 'text-green-500' : 'text-red-500'}`}>
                                    {stat.change}
                                </span>
                            </div>
                            <h3 className="text-gray-400 text-sm font-medium">{stat.title}</h3>
                            <p className="text-2xl font-bold text-gray-800 mt-1">{stat.value}</p>
                        </div>
                    );
                })}
            </div>

            {/* Placeholder for Recent Activity Table */}
            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <div className="flex items-center justify-between mb-6">
                    <h2 className="text-xl font-bold text-gray-800">أحدث الطلبات</h2>
                    <button className="text-primary hover:underline font-medium text-sm">عرض الكل</button>
                </div>
                <div className="text-center py-20 text-gray-400 border-2 border-dashed border-gray-100 rounded-xl">
                    <p>سيتم عرض قائمة الطلبات الأخيرة هنا بمجرد ربطها بقاعدة البيانات.</p>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
