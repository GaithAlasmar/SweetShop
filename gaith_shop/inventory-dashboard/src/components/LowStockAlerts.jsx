import React from 'react';
import { FaExclamationTriangle } from 'react-icons/fa';

const LowStockAlerts = ({ products }) => {
    const lowStockProducts = products.filter(p => p.stock < p.minStock);

    const getStockLevel = (current, min) => {
        const percentage = (current / min) * 100;
        if (percentage <= 30) return 'critical';
        if (percentage <= 60) return 'warning';
        return 'normal';
    };

    return (
        <div className="chart-card">
            <div className="chart-header">
                <h2>تنبيهات المخزون</h2>
                <p className="chart-subtitle">{lowStockProducts.length} منتج يحتاج إلى إعادة تعبئة</p>
            </div>
            <div className="alerts-container">
                {lowStockProducts.length === 0 ? (
                    <div className="empty-state">
                        <p>جميع المنتجات متوفرة بكميات كافية 🎉</p>
                    </div>
                ) : (
                    lowStockProducts.map((product) => {
                        const level = getStockLevel(product.stock, product.minStock);
                        return (
                            <div key={product.id} className={`alert-card ${level}`}>
                                <div className="alert-icon">
                                    <FaExclamationTriangle size={20} />
                                </div>
                                <div className="alert-content">
                                    <h4>{product.name}</h4>
                                    <p className="alert-category">{product.category}</p>
                                    <div className="stock-info">
                                        <span className="current-stock">الكمية الحالية: <strong>{product.stock}</strong></span>
                                        <span className="min-stock">الحد الأدنى: {product.minStock}</span>
                                    </div>
                                    <div className="stock-bar">
                                        <div
                                            className="stock-progress"
                                            style={{ width: `${(product.stock / product.minStock) * 100}%` }}
                                        />
                                    </div>
                                </div>
                                <div className="alert-badge">
                                    {level === 'critical' ? 'حرج' : 'تحذير'}
                                </div>
                            </div>
                        );
                    })
                )}
            </div>
        </div>
    );
};

export default LowStockAlerts;
