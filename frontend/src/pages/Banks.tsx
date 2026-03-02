import React, { useState } from 'react';
import { Card, Table, Button, Space, Modal, Form, Input, notification } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import api from '../api/axios';

interface Bank {
    id: string;
    name: string;
    swiftCode?: string;
}

const Banks: React.FC = () => {
    const queryClient = useQueryClient();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [form] = Form.useForm();

    const { data, isLoading } = useQuery({
        queryKey: ['banks'],
        queryFn: async () => {
            const response = await api.get<Bank[]>('/banks');
            return response.data;
        },
    });

    const createMutation = useMutation({
        mutationFn: (values: Omit<Bank, 'id'>) => api.post('/banks', values),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['banks'] });
            setIsModalOpen(false);
            form.resetFields();
            notification.success({ message: 'Bank created successfully' });
        }
    });

    const updateMutation = useMutation({
        mutationFn: (variables: Bank) => api.put(`/banks/${variables.id}`, { name: variables.name, swiftCode: variables.swiftCode }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['banks'] });
            setIsModalOpen(false);
            setEditingId(null);
            form.resetFields();
            notification.success({ message: 'Bank updated successfully' });
        }
    });

    const deleteMutation = useMutation({
        mutationFn: (id: string) => api.delete(`/banks/${id}`),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['banks'] });
            notification.success({ message: 'Bank deleted successfully' });
        }
    });

    const handleSubmit = (values: Omit<Bank, 'id'>) => {
        if (editingId) {
            updateMutation.mutate({ id: editingId, ...values });
        } else {
            createMutation.mutate(values);
        }
    };

    const columns = [
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'SWIFT Code', dataIndex: 'swiftCode', key: 'swiftCode' },
        {
            title: 'Actions',
            key: 'actions',
            render: (_: any, record: Bank) => (
                <Space>
                    <Button type="text" icon={<EditOutlined />} onClick={() => { setEditingId(record.id); form.setFieldsValue(record); setIsModalOpen(true); }} />
                    <Button
                        type="text"
                        danger
                        icon={<DeleteOutlined />}
                        onClick={() => {
                            Modal.confirm({
                                title: 'Delete Bank',
                                content: `Are you sure you want to delete ${record.name}?`,
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
                <h2 className="text-xl font-bold m-0">Banks</h2>
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditingId(null); form.resetFields(); setIsModalOpen(true); }}>
                    Add Bank
                </Button>
            </div>

            <Card className="shadow-sm">
                <Table columns={columns} dataSource={data} rowKey="id" loading={isLoading} pagination={{ pageSize: 15 }} />
            </Card>

            <Modal
                title={editingId ? 'Edit Bank' : 'New Bank'}
                open={isModalOpen}
                onCancel={() => { setIsModalOpen(false); form.resetFields(); setEditingId(null); }}
                onOk={() => form.submit()}
                confirmLoading={createMutation.isPending || updateMutation.isPending}
            >
                <Form form={form} layout="vertical" onFinish={handleSubmit}>
                    <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Please enter a matching Bank name' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="swiftCode" label="SWIFT Code">
                        <Input />
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default Banks;
