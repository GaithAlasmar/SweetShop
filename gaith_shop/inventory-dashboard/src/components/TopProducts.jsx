import React from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Cell } from 'recharts';

const TopProducts = ({ products }) => {
    // الحصول على أفضل 5 منتجات حسب المبيعات
    const topProducts = [...products]
        .sort((a, b) => b.sales - a.sales)
        .slice(0, 5);

    const colors = ['#3b82f6', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981'];

    return (
        <div className="chart-card">
            <div className="chart-header">
                <h2>المنتجات الأكثر مبيعاً</h2>
                <p className="chart-subtitle">أفضل 5 منتجات هذا الشهر</p>
            </div>
            <div className="chart-container">
                <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={topProducts} layout="vertical">
                        <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                        <XAxis type="number" stroke="#6b7280" />
                        <YAxis dataKey="name" type="category" width={150} stroke="#6b7280" />
                        <Tooltip
                            contentStyle={{
                                background: '#fff',
                                border: '1px solid #e5e7eb',
                                borderRadius: '8px',
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
                            }}
                            formatter={(value) => [`${value} وحدة`, 'المبيعات']}
                        />
                        <Bar dataKey="sales" radius={[0, 8, 8, 0]}>
                            {topProducts.map((entry, index) => (
                                <Cell key={`cell-${index}`} fill={colors[index]} />
                            ))}
                        </Bar>
                    </BarChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};

export default TopProducts;
