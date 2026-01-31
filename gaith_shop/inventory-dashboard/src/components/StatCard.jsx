import React from 'react';
import { TrendingUp, TrendingDown, Package, DollarSign } from 'react-icons/fa';

const StatCard = ({ title, value, icon: Icon, trend, trendValue, colorClass }) => {
    const isPositive = trend === 'up';

    return (
        <div className="stat-card">
            <div className="stat-card-header">
                <div className="stat-icon" style={{ background: colorClass }}>
                    <Icon size={24} />
                </div>
                {trendValue && (
                    <div className={`stat-trend ${isPositive ? 'positive' : 'negative'}`}>
                        {isPositive ? <TrendingUp size={16} /> : <TrendingDown size={16} />}
                        <span>{trendValue}%</span>
                    </div>
                )}
            </div>
            <div className="stat-content">
                <h3 className="stat-title">{title}</h3>
                <p className="stat-value">{value}</p>
            </div>
        </div>
    );
};

export default StatCard;
