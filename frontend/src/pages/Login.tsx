import React, { useState } from 'react';
import { Card, Form, Input, Button, Tabs } from 'antd';
import { useTranslation } from 'react-i18next';
import { useNavigate as useAppNavigate } from 'react-router-dom';
import api from '../api/axios';

const { TabPane } = Tabs;

const Login: React.FC = () => {
    const { t } = useTranslation();
    const navigate = useAppNavigate();
    const [loading, setLoading] = useState(false);
    const [activeTab, setActiveTab] = useState('login');
    const [form] = Form.useForm();

    const onFinish = async (values: any) => {
        setLoading(true);
        try {
            if (activeTab === 'login') {
                const response = await api.post('/auth/login', {
                    username: values.username,
                    password: values.password,
                });

                const { token } = response.data;
                localStorage.setItem('token', token);

                const redirectUrl = localStorage.getItem('redirectUrl') || '/dashboard';
                localStorage.removeItem('redirectUrl');
                navigate(redirectUrl, { replace: true });
            } else {
                // Register flow
                await api.post('/auth/register', {
                    username: values.username,
                    password: values.password,
                    fullName: values.fullName,
                });

                // Auto-login after registration
                const loginResponse = await api.post('/auth/login', {
                    username: values.username,
                    password: values.password,
                });
                const { token } = loginResponse.data;
                localStorage.setItem('token', token);

                navigate('/dashboard', { replace: true });
            }
        } catch (error) {
            console.error("Auth failed", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50">
            <Card className="w-full max-w-md shadow-lg rounded-xl border-0">
                <div className="text-center mb-8">
                    <h1 className="text-3xl font-bold text-primary mb-2">MoneyTrace</h1>
                    <p className="text-gray-500">{t('welcome')}</p>
                </div>

                <Tabs activeKey={activeTab} onChange={(key) => { setActiveTab(key); form.resetFields(); }} centered>
                    <TabPane tab="Login" key="login">
                        <Form
                            form={form}
                            name="login_form"
                            layout="vertical"
                            onFinish={onFinish}
                            autoComplete="off"
                        >
                            <Form.Item
                                label="Username"
                                name="username"
                                rules={[{ required: true, message: 'Please input your username!' }]}
                            >
                                <Input size="large" />
                            </Form.Item>

                            <Form.Item
                                label="Password"
                                name="password"
                                rules={[{ required: true, message: 'Please input your password!' }]}
                            >
                                <Input.Password size="large" />
                            </Form.Item>

                            <Form.Item>
                                <Button
                                    type="primary"
                                    htmlType="submit"
                                    size="large"
                                    className="w-full mt-4"
                                    loading={loading}
                                >
                                    {t('login')}
                                </Button>
                            </Form.Item>
                        </Form>
                    </TabPane>
                    <TabPane tab="Register" key="register">
                        <Form
                            form={form}
                            name="register_form"
                            layout="vertical"
                            onFinish={onFinish}
                            autoComplete="off"
                        >
                            <Form.Item
                                label="Username"
                                name="username"
                                rules={[{ required: true, message: 'Please input your username!' }]}
                            >
                                <Input size="large" />
                            </Form.Item>

                            <Form.Item
                                label="Full Name"
                                name="fullName"
                                rules={[{ required: true, message: 'Please input your full name!' }]}
                            >
                                <Input size="large" />
                            </Form.Item>

                            <Form.Item
                                label="Password"
                                name="password"
                                rules={[{ required: true, message: 'Please input your password!' }]}
                            >
                                <Input.Password size="large" />
                            </Form.Item>

                            <Form.Item>
                                <Button
                                    type="primary"
                                    htmlType="submit"
                                    size="large"
                                    className="w-full mt-4"
                                    loading={loading}
                                >
                                    Register
                                </Button>
                            </Form.Item>
                        </Form>
                    </TabPane>
                </Tabs>
            </Card>
        </div>
    );
};

export default Login;
