import React, { useState, useMemo } from 'react';
import { useLanguage } from './context/LanguageContext';
import LanguageToggle from './components/LanguageToggle';
import { FaBox, FaExclamationTriangle, FaDollarSign, FaChartLine } from 'react-icons/fa';
import StatCard from './components/StatCard';
import TopProducts from './components/TopProducts';
import LowStockAlerts from './components/LowStockAlerts';
import FilterPanel from './components/FilterPanel';
import SalesChart from './components/SalesChart';
import CategoryChart from './components/CategoryChart';
import { inventoryData } from './data/mockData';
import './App.css';

function App() {
  const { t, direction } = useLanguage();

  const [filters, setFilters] = useState({
    category: 'all',
    status: 'الكل',
    priceRange: 'all'
  });

  // منطق الفلترة
  const filteredProducts = useMemo(() => {
    return inventoryData.products.filter(product => {
      // Category Filter
      if (filters.category !== 'all' && product.category !== filters.category) {
        return false;
      }

      // فلتر الحالة
      if (filters.status !== 'الكل' && product.status !== filters.status) {
        return false;
      }

      // فلتر السعر
      if (filters.priceRange !== 'all') {
        const price = product.price;
        switch (filters.priceRange) {
          case '0-200':
            if (price >= 200) return false;
            break;
          case '200-500':
            if (price < 200 || price > 500) return false;
            break;
          case '500-1000':
            if (price < 500 || price > 1000) return false;
            break;
          case '1000+':
            if (price < 1000) return false;
            break;
        }
      }

      return true;
    });
  }, [filters]);

  const handleFilterChange = (filterType, value) => {
    if (filterType === 'reset') {
      setFilters({
        category: 'all',
        status: 'all',
        priceRange: 'all'
      });
    } else {
      setFilters(prev => ({
        ...prev,
        [filterType]: value
      }));
    }
  };

  // حساب الإحصائيات
  const stats = {
    totalProducts: filteredProducts.length,
    lowStock: filteredProducts.filter(p => p.stock < p.minStock).length,
    totalRevenue: filteredProducts.reduce((sum, p) => sum + (p.price * p.sales), 0),
    monthlySalesGrowth: inventoryData.stats.monthlySalesGrowth
  };

  return (
    <div className="app" dir={direction}>
      <header className="dashboard-header">
        <div className="header-content">
          <div className="header-top">
            <h1>{t('appTitle')}</h1>
            <LanguageToggle />
          </div>
          <p className="header-subtitle">{t('appSubtitle')}</p>
        </div>
      </header>

      <div className="dashboard-container">
        <aside className="sidebar">
          <FilterPanel filters={filters} onFilterChange={handleFilterChange} />
        </aside>

        <main className="main-content">
          {/* بطاقات الإحصائيات */}
          <section className="stats-grid">
            <StatCard
              title={t('totalProducts')}
              value={stats.totalProducts}
              icon={FaBox}
              trend="up"
              trendValue={8}
              colorClass="linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
            />
            <StatCard
              title={t('lowStock')}
              value={stats.lowStock}
              icon={FaExclamationTriangle}
              trend="down"
              trendValue={3}
              colorClass="linear-gradient(135deg, #f093fb 0%, #f5576c 100%)"
            />
            <StatCard
              title={t('totalRevenue')}
              value={`${stats.totalRevenue.toLocaleString()} ${t('currency')}`}
              icon={FaDollarSign}
              trend="up"
              trendValue={stats.monthlySalesGrowth}
              colorClass="linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)"
            />
            <StatCard
              title={t('salesGrowth')}
              value={`${stats.monthlySalesGrowth}%`}
              icon={FaChartLine}
              trend="up"
              trendValue={stats.monthlySalesGrowth}
              colorClass="linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)"
            />
          </section>

          {/* الرسوم البيانية */}
          <section className="charts-grid">
            <div className="chart-row">
              <SalesChart data={inventoryData.monthlySales} />
              <CategoryChart data={inventoryData.categoryDistribution} />
            </div>
            <div className="chart-row">
              <TopProducts products={filteredProducts} />
              <LowStockAlerts products={filteredProducts} />
            </div>
          </section>
        </main>
      </div>
    </div>
  );
}

export default App;
