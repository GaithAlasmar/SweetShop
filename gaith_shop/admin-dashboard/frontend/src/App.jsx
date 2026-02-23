import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useState } from 'react';
import Sidebar from './components/Sidebar';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';
import Products from './pages/Products';
import Settings from './pages/Settings';

const App = () => {
    const [isAdmin, setIsAdmin] = useState(true); // Placeholder for auth logic

    if (!isAdmin) {
        return (
            <div className="flex items-center justify-center h-screen bg-bgLight">
                <div className="text-center">
                    <h1 className="text-4xl font-bold text-primary">403 - Forbidden</h1>
                    <p className="mt-4 text-gray-600">You do not have permission to access the admin panel.</p>
                    <button
                        onClick={() => window.location.href = '/'}
                        className="mt-6 px-6 py-2 bg-primary text-white rounded-lg"
                    >
                        Back to Shop
                    </button>
                </div>
            </div>
        );
    }

    return (
        <Router>
            <div className="flex min-h-screen bg-bgLight w-full">
                <Sidebar />
                <main className="flex-1 p-8 overflow-y-auto">
                    <Routes>
                        <Route path="/admin" element={<Dashboard />} />
                        <Route path="/admin/users" element={<Users />} />
                        <Route path="/admin/products" element={<Products />} />
                        <Route path="/admin/settings" element={<Settings />} />
                        <Route path="*" element={<Navigate to="/admin" />} />
                    </Routes>
                </main>
            </div>
        </Router>
    );
};

export default App;
