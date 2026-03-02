import React, { useState } from 'react';
import { Card, Table, Button, Space, Modal, Form, Input, InputNumber, notification } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import api from '../api/axios';

interface Currency {
    id: string;
    code: string;
    symbol?: string;
    name?: string;
    rateToDefault: number;
}

const Currencies: React.FC = () => {
    const queryClient = useQueryClient();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [form] = Form.useForm();

    const { data, isLoading } = useQuery({
        queryKey: ['currencies'],
        queryFn: async () => {
            const response = await api.get<Currency[]>('/currencies');
            return response.data;
        },
    });

    const createMutation = useMutation({
        mutationFn: (values: Omit<Currency, 'id'>) => api.post('/currencies', values),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            setIsModalOpen(false);
            form.resetFields();
            notification.success({ message: 'Currency created successfully' });
        }
    });

    const updateMutation = useMutation({
        mutationFn: (variables: Currency) => api.put(`/currencies/${variables.id}`, { code: variables.code, symbol: variables.symbol, name: variables.name, rateToDefault: variables.rateToDefault }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            setIsModalOpen(false);
            setEditingId(null);
            form.resetFields();
            notification.success({ message: 'Currency updated successfully' });
        }
    });

    const deleteMutation = useMutation({
        mutationFn: (id: string) => api.delete(`/currencies/${id}`),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            notification.success({ message: 'Currency deleted successfully' });
        }
    });

    const handleSubmit = (values: Omit<Currency, 'id'>) => {
        if (editingId) {
            updateMutation.mutate({ id: editingId, ...values });
        } else {
            createMutation.mutate(values);
        }
    };

    const columns = [
        { title: 'Code', dataIndex: 'code', key: 'code' },
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'Symbol', dataIndex: 'symbol', key: 'symbol' },
        { title: 'Rate (vs Default)', dataIndex: 'rateToDefault', key: 'rateToDefault' },
        {
            title: 'Actions',
            key: 'actions',
            render: (_: any, record: Currency) => (
                <Space>
                    <Button type="text" icon={<EditOutlined />} onClick={() => { setEditingId(record.id); form.setFieldsValue(record); setIsModalOpen(true); }} />
                    <Button
                        type="text"
                        danger
                        icon={<DeleteOutlined />}
                        onClick={() => {
                            Modal.confirm({
                                title: 'Delete Currency',
                                content: `Are you sure you want to delete ${record.code}?`,
                                onOk: () => deleteMutation.mutate(record.id),
                            });
                        }}
                    />
                </Space>
            ),
        }
    ];

    return (
        <div>
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold m-0">Currencies</h2>
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditingId(null); form.resetFields(); setIsModalOpen(true); }}>
                    Add Currency
                </Button>
            </div>

            <Card className="shadow-sm">
                <Table columns={columns} dataSource={data} rowKey="id" loading={isLoading} pagination={{ pageSize: 15 }} />
            </Card>

            <Modal
                title={editingId ? 'Edit Currency' : 'New Currency'}
                open={isModalOpen}
                onCancel={() => { setIsModalOpen(false); form.resetFields(); setEditingId(null); }}
                onOk={() => form.submit()}
                confirmLoading={createMutation.isPending || updateMutation.isPending}
            >
                <Form form={form} layout="vertical" onFinish={handleSubmit}>
                    <Form.Item name="code" label="Code (e.g. USD)" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="name" label="Name (e.g. US Dollar)">
                        <Input />
                    </Form.Item>
                    <Form.Item name="symbol" label="Symbol (e.g. $)">
                        <Input />
                    </Form.Item>
                    <Form.Item name="rateToDefault" label="Rate to Default Currency" rules={[{ required: true }]}>
                        <InputNumber min={0.000001} step={0.01} className="w-full" />
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default Currencies;
