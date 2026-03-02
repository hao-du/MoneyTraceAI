import React from 'react';
import { Layout, Menu, Button } from 'antd';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { PieChartOutlined, TransactionOutlined, SettingOutlined, LogoutOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

const { Header, Content, Sider } = Layout;

const MainLayout: React.FC = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    const menuItems = [
        {
            key: '/dashboard',
            icon: <PieChartOutlined />,
            label: t('dashboard'),
        },
        {
            key: '/transactions',
            icon: <TransactionOutlined />,
            label: t('transactions'),
        },
        {
            key: '/counterparties',
            icon: <TransactionOutlined />,
            label: 'Counterparties',
        },
        {
            key: '/banks',
            icon: <TransactionOutlined />,
            label: 'Banks',
        },
        {
            key: '/currencies',
            icon: <TransactionOutlined />,
            label: 'Currencies',
        },
        {
            key: '/settings',
            icon: <SettingOutlined />,
            label: t('settings'),
        },
    ];

    return (
        <Layout className="min-h-screen">
            <Sider breakpoint="lg" collapsedWidth="0" theme="light" className="shadow-md z-10">
                <div className="h-16 flex items-center justify-center font-bold text-xl text-primary border-b border-gray-100">
                    MoneyTrace
                </div>
                <Menu
                    theme="light"
                    mode="inline"
                    selectedKeys={[location.pathname]}
                    items={menuItems}
                    onClick={({ key }) => navigate(key)}
                    className="mt-4 border-r-0"
                />
                <div className="absolute bottom-4 w-full px-4">
                    <Button type="text" danger block icon={<LogoutOutlined />} onClick={handleLogout} className="text-left">
                        {t('logout')}
                    </Button>
                </div>
            </Sider>
            <Layout>
                <Header className="bg-white px-6 flex items-center shadow-sm z-0">
                    <h1 className="text-xl font-semibold m-0">{menuItems.find(m => m.key === location.pathname)?.label || 'MoneyTrace'}</h1>
                </Header>
                <Content className="m-6 p-6 min-h-[280px] bg-white rounded-lg shadow-sm overflow-auto">
                    <Outlet />
                </Content>
            </Layout>
        </Layout>
    );
};

export default MainLayout;
