import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';

const SalesChart = ({ data }) => {
    return (
        <div className="chart-card">
            <div className="chart-header">
                <h2>المبيعات الشهرية</h2>
                <p className="chart-subtitle">مقارنة المبيعات والمصروفات</p>
            </div>
            <div className="chart-container">
                <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={data}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                        <XAxis dataKey="month" stroke="#6b7280" />
                        <YAxis stroke="#6b7280" />
                        <Tooltip
                            contentStyle={{
                                background: '#fff',
                                border: '1px solid #e5e7eb',
                                borderRadius: '8px',
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
                            }}
                            formatter={(value) => `${value.toLocaleString()} ريال`}
                        />
                        <Legend />
                        <Line
                            type="monotone"
                            dataKey="sales"
                            stroke="#3b82f6"
                            strokeWidth={3}
                            name="المبيعات"
                            dot={{ fill: '#3b82f6', r: 5 }}
                        />
                        <Line
                            type="monotone"
                            dataKey="expenses"
                            stroke="#ef4444"
                            strokeWidth={3}
                            name="المصروفات"
                            dot={{ fill: '#ef4444', r: 5 }}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};

export default SalesChart;
