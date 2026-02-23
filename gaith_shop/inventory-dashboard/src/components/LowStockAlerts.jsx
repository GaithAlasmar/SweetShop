import React from 'react';
import { FaExclamationTriangle } from 'react-icons/fa';
import { useLanguage } from '../context/LanguageContext';

const LowStockAlerts = ({ products }) => {
    const { t } = useLanguage();
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
                <h2>{t('lowStockAlerts')}</h2>
                <p className="chart-subtitle">{lowStockProducts.length} {t('needsRestock')}</p>
            </div>
            <div className="alerts-container">
                {lowStockProducts.length === 0 ? (
                    <div className="empty-state">
                        <p>{t('allStockOk')}</p>
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
                                    <h4>{t(product.name) || product.name}</h4>
                                    <p className="alert-category">{t(product.category) || product.category}</p>
                                    <div className="stock-info">
                                        <span className="current-stock">{t('currentStock')}: <strong>{product.stock}</strong></span>
                                        <span className="min-stock">{t('minStockLabel')}: {product.minStock}</span>
                                    </div>
                                    <div className="stock-bar">
                                        <div
                                            className="stock-progress"
                                            style={{ width: `${(product.stock / product.minStock) * 100}%` }}
                                        />
                                    </div>
                                </div>
                                <div className="alert-badge">
                                    {level === 'critical' ? t('critical') : t('warning')}
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
