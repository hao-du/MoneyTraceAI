import React, { useEffect } from 'react';
import { Card, Form, Button, notification, Select } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import api from '../api/axios';

const { Option } = Select;

interface Setting {
    id: string;
    defaultCurrencyId?: string;
    language: string;
    theme: string;
}

const Settings: React.FC = () => {
    const { t, i18n } = useTranslation();
    const queryClient = useQueryClient();
    const [form] = Form.useForm();

    const { data, isLoading } = useQuery({
        queryKey: ['settings'],
        queryFn: async () => {
            const response = await api.get<Setting>('/settings');
            return response.data;
        },
    });

    const { data: currencies } = useQuery({
        queryKey: ['currencies'],
        queryFn: async () => {
            const response = await api.get<{ id: string, code: string }[]>('/currencies');
            return response.data;
        },
    });

    useEffect(() => {
        if (data) {
            form.setFieldsValue(data);
        }
    }, [data, form]);

    const saveMutation = useMutation({
        mutationFn: (values: Omit<Setting, 'id'>) => api.post('/settings', values),
        onSuccess: (response, variables) => {
            queryClient.invalidateQueries({ queryKey: ['settings'] });
            notification.success({ message: 'Settings saved successfully' });
            // Apply language change instantly if altered
            if (variables.language) {
                i18n.changeLanguage(variables.language);
            }
        }
    });

    const onFinish = (values: any) => {
        saveMutation.mutate(values);
    };

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <h2 className="text-xl font-bold m-0">{t('settings')}</h2>

            <Card className="shadow-sm" loading={isLoading}>
                <Form
                    form={form}
                    layout="vertical"
                    onFinish={onFinish}
                    initialValues={{ language: 'en', theme: 'light' }}
                >
                    <Form.Item label="Default Currency" name="defaultCurrencyId">
                        <Select placeholder="Select a default currency" allowClear>
                            {currencies?.map(c => (
                                <Option key={c.id} value={c.id}>{c.code}</Option>
                            ))}
                        </Select>
                    </Form.Item>

                    <Form.Item label="Language" name="language">
                        <Select>
                            <Option value="en">English</Option>
                            <Option value="vi">Tiếng Việt</Option>
                        </Select>
                    </Form.Item>

                    <Form.Item label="Theme" name="theme">
                        <Select>
                            <Option value="light">Light</Option>
                            <Option value="dark">Dark (Coming Soon)</Option>
                        </Select>
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" loading={saveMutation.isPending}>
                            Save Settings
                        </Button>
                    </Form.Item>
                </Form>
            </Card>
        </div>
    );
};

export default Settings;
