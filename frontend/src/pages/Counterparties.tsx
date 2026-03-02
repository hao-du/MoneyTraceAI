import React, { useState } from 'react';
import { Card, Table, Button, Space, Modal, Form, Input, notification } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import api from '../api/axios';

interface Counterparty {
    id: string;
    name: string;
}

const Counterparties: React.FC = () => {
    const queryClient = useQueryClient();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [form] = Form.useForm();

    // Fetch
    const { data, isLoading } = useQuery({
        queryKey: ['counterparties'],
        queryFn: async () => {
            const response = await api.get<Counterparty[]>('/counterparties');
            return response.data;
        },
    });

    // Create
    const createMutation = useMutation({
        mutationFn: (name: string) => api.post('/counterparties', { name }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['counterparties'] });
            setIsModalOpen(false);
            form.resetFields();
            notification.success({ message: 'Counterparty created successfully' });
        }
    });

    // Update
    const updateMutation = useMutation({
        mutationFn: (variables: { id: string, name: string }) => api.put(`/counterparties/${variables.id}`, { name: variables.name }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['counterparties'] });
            setIsModalOpen(false);
            setEditingId(null);
            form.resetFields();
            notification.success({ message: 'Counterparty updated successfully' });
        }
    });

    // Delete
    const deleteMutation = useMutation({
        mutationFn: (id: string) => api.delete(`/counterparties/${id}`),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['counterparties'] });
            notification.success({ message: 'Counterparty deleted successfully' });
        }
    });

    const handleSubmit = (values: { name: string }) => {
        if (editingId) {
            updateMutation.mutate({ id: editingId, name: values.name });
        } else {
            createMutation.mutate(values.name);
        }
    };

    const openEditModal = (record: Counterparty) => {
        setEditingId(record.id);
        form.setFieldsValue({ name: record.name });
        setIsModalOpen(true);
    };

    const openCreateModal = () => {
        setEditingId(null);
        form.resetFields();
        setIsModalOpen(true);
    };

    const columns = [
        { title: 'Name', dataIndex: 'name', key: 'name' },
        {
            title: 'Actions',
            key: 'actions',
            render: (_: any, record: Counterparty) => (
                <Space>
                    <Button type="text" icon={<EditOutlined />} onClick={() => openEditModal(record)} />
                    <Button
                        type="text"
                        danger
                        icon={<DeleteOutlined />}
                        onClick={() => {
                            Modal.confirm({
                                title: 'Delete Counterparty',
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
                <h2 className="text-xl font-bold m-0">Counterparties</h2>
                <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
                    Add Counterparty
                </Button>
            </div>

            <Card className="shadow-sm">
                <Table
                    columns={columns}
                    dataSource={data}
                    rowKey="id"
                    loading={isLoading}
                    pagination={{ pageSize: 15 }}
                />
            </Card>

            <Modal
                title={editingId ? 'Edit Counterparty' : 'New Counterparty'}
                open={isModalOpen}
                onCancel={() => { setIsModalOpen(false); form.resetFields(); setEditingId(null); }}
                onOk={() => form.submit()}
                confirmLoading={createMutation.isPending || updateMutation.isPending}
            >
                <Form form={form} layout="vertical" onFinish={handleSubmit}>
                    <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Please enter a name' }]}>
                        <Input />
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default Counterparties;
