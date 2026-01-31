// بيانات تجريبية للمخزون
export const inventoryData = {
    products: [
        {
            id: 1,
            name: 'لابتوب HP',
            category: 'إلكترونيات',
            stock: 5,
            price: 3500,
            sales: 45,
            status: 'منخفض',
            minStock: 10
        },
        {
            id: 2,
            name: 'ماوس Logitech',
            category: 'ملحقات',
            stock: 150,
            price: 120,
            sales: 89,
            status: 'متوفر',
            minStock: 50
        },
        {
            id: 3,
            name: 'لوحة مفاتيح ميكانيكية',
            category: 'ملحقات',
            stock: 8,
            price: 450,
            sales: 67,
            status: 'منخفض',
            minStock: 15
        },
        {
            id: 4,
            name: 'شاشة Samsung 27"',
            category: 'إلكترونيات',
            stock: 25,
            price: 1200,
            sales: 34,
            status: 'متوفر',
            minStock: 10
        },
        {
            id: 5,
            name: 'طابعة Canon',
            category: 'إلكترونيات',
            stock: 12,
            price: 800,
            sales: 23,
            status: 'متوفر',
            minStock: 8
        },
        {
            id: 6,
            name: 'سماعات Bluetooth',
            category: 'ملحقات',
            stock: 3,
            price: 250,
            sales: 156,
            status: 'منخفض',
            minStock: 20
        },
        {
            id: 7,
            name: 'كاميرا ويب HD',
            category: 'ملحقات',
            stock: 45,
            price: 180,
            sales: 78,
            status: 'متوفر',
            minStock: 15
        },
        {
            id: 8,
            name: 'حقيبة لابتوب',
            category: 'إكسسوارات',
            stock: 2,
            price: 90,
            sales: 92,
            status: 'منخفض',
            minStock: 25
        }
    ],

    monthlySales: [
        { month: 'يناير', sales: 45000, expenses: 32000 },
        { month: 'فبراير', sales: 52000, expenses: 38000 },
        { month: 'مارس', sales: 48000, expenses: 35000 },
        { month: 'أبريل', sales: 61000, expenses: 42000 },
        { month: 'مايو', sales: 55000, expenses: 40000 },
        { month: 'يونيو', sales: 67000, expenses: 45000 }
    ],

    categoryDistribution: [
        { name: 'إلكترونيات', value: 45, color: '#3b82f6' },
        { name: 'ملحقات', value: 35, color: '#8b5cf6' },
        { name: 'إكسسوارات', value: 20, color: '#ec4899' }
    ],

    stats: {
        totalProducts: 250,
        lowStock: 4,
        totalRevenue: 328000,
        monthlySalesGrowth: 12.5
    }
};
