import { createBrowserRouter, Navigate } from 'react-router-dom';
import MainLayout from '../components/layouts/MainLayout';
import Login from '../pages/Login';
import Dashboard from '../pages/Dashboard';
import Transactions from '../pages/Transactions';
import Counterparties from '../pages/Counterparties';
import Banks from '../pages/Banks';
import Currencies from '../pages/Currencies';
import Settings from '../pages/Settings';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const token = localStorage.getItem('token');
    if (!token) {
        localStorage.setItem('redirectUrl', window.location.pathname);
        return <Navigate to="/login" replace />;
    }
    return <>{children}</>;
};

export const router = createBrowserRouter([
    {
        path: '/login',
        element: <Login />,
    },
    {
        path: '/',
        element: (
            <ProtectedRoute>
                <MainLayout />
            </ProtectedRoute>
        ),
        children: [
            {
                index: true,
                element: <Navigate to="/dashboard" replace />,
            },
            {
                path: 'dashboard',
                element: <Dashboard />,
            },
            {
                path: 'transactions',
                element: <Transactions />,
            },
            {
                path: 'counterparties',
                element: <Counterparties />,
            },
            {
                path: 'banks',
                element: <Banks />,
            },
            {
                path: 'currencies',
                element: <Currencies />,
            },
            {
                path: 'settings',
                element: <Settings />,
            },
            // Other pages will go here
        ],
    },
]);
