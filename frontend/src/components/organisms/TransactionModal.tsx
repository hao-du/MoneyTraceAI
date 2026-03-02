import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, Select, Radio, DatePicker, message } from 'antd';
import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import api from '../../api/axios';

const { Option } = Select;
const { TextArea } = Input;

type TransactionFormType = 'cashflow' | 'bank' | 'exchange' | 'transfer';

interface TransactionModalProps {
    open: boolean;
    onClose: () => void;
}

const TransactionModal: React.FC<TransactionModalProps> = ({ open, onClose }) => {
    const [form] = Form.useForm();
    const queryClient = useQueryClient();
    const [type, setType] = useState<TransactionFormType>('cashflow');

    const { data: currencies } = useQuery({ queryKey: ['currencies'], queryFn: async () => (await api.get<{ id: string, code: string }[]>('/currencies')).data });
    const { data: banks } = useQuery({ queryKey: ['banks'], queryFn: async () => (await api.get<{ id: string, name: string }[]>('/banks')).data });
    const { data: counterparties } = useQuery({ queryKey: ['counterparties'], queryFn: async () => (await api.get<{ id: string, name: string }[]>('/counterparties')).data });

    const cashflowMutation = useMutation({ mutationFn: (values: any) => api.post('/transactions/cashflow', values), onSuccess: () => handleSuccess() });
    const bankMutation = useMutation({ mutationFn: (values: any) => api.post('/transactions/bank', values), onSuccess: () => handleSuccess() });
    const exchangeMutation = useMutation({ mutationFn: (values: any) => api.post('/transactions/exchange', values), onSuccess: () => handleSuccess() });
    const transferMutation = useMutation({ mutationFn: (values: any) => api.post('/transactions/transfer', values), onSuccess: () => handleSuccess() });

    const handleSuccess = () => {
        queryClient.invalidateQueries({ queryKey: ['transactions_list'] });
        queryClient.invalidateQueries({ queryKey: ['dashboard_transactions'] });
        message.success('Transaction created successfully!');
        onClose();
        form.resetFields();
    };

    const handleSubmit = (values: any) => {
        // Transform dates to UTC strings as expected by backend Guid V7 and UTC requirement
        const basePayload = {
            dateUtc: values.date.toISOString(),
            amount: values.amount,
            currencyId: values.currencyId,
            description: values.description,
            tags: values.tags,
        };

        if (type === 'cashflow') {
            cashflowMutation.mutate({ ...basePayload, isIncome: values.isIncome });
        } else if (type === 'bank') {
            bankMutation.mutate({
                ...basePayload,
                bankId: values.bankId,
                accountNumber: values.accountNumber,
                interestPercentage: values.interestPercentage || 0,
                interestPeriod: values.interestPeriod || 0,
                interestAmount: 0,
                actualInterestAmount: 0
            });
        } else if (type === 'exchange') {
            exchangeMutation.mutate({
                ...basePayload,
                sourceAmount: values.amount,
                sourceCurrencyId: values.currencyId,
                targetAmount: values.targetAmount,
                targetCurrencyId: values.targetCurrencyId,
                exchangeRate: values.exchangeRate
            });
        } else if (type === 'transfer') {
            transferMutation.mutate({
                ...basePayload,
                counterpartyId: values.counterpartyId,
                transferType: values.transferType,
                status: 0 // Pending
            });
        }
    };

    const isPending = cashflowMutation.isPending || bankMutation.isPending || exchangeMutation.isPending || transferMutation.isPending;

    return (
        <Modal
            title="New Transaction"
            open={open}
            onCancel={onClose}
            onOk={() => form.submit()}
            confirmLoading={isPending}
            width={600}
            destroyOnClose
        >
            <Form
                form={form}
                layout="vertical"
                onFinish={handleSubmit}
                initialValues={{ type: 'cashflow', isIncome: false, date: dayjs() }}
                onValuesChange={(changedValues) => {
                    if (changedValues.type) setType(changedValues.type);
                }}
            >
                <Form.Item name="type" label="Transaction Type">
                    <Radio.Group optionType="button" buttonStyle="solid">
                        <Radio.Button value="cashflow">Cashflow</Radio.Button>
                        <Radio.Button value="bank">Bank</Radio.Button>
                        <Radio.Button value="exchange">Exchange</Radio.Button>
                        <Radio.Button value="transfer">Transfer</Radio.Button>
                    </Radio.Group>
                </Form.Item>

                <div className="grid grid-cols-2 gap-4">
                    <Form.Item name="date" label="Date" rules={[{ required: true }]}>
                        <DatePicker showTime className="w-full" />
                    </Form.Item>

                    <Form.Item name="currencyId" label="Currency" rules={[{ required: true }]}>
                        <Select showSearch optionFilterProp="children">
                            {currencies?.map(c => <Option key={c.id} value={c.id}>{c.code}</Option>)}
                        </Select>
                    </Form.Item>
                </div>

                {/* Dynamic Fields based on Type */}
                {type === 'cashflow' && (
                    <div className="grid grid-cols-2 gap-4">
                        <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                            <InputNumber min={0.01} className="w-full" />
                        </Form.Item>
                        <Form.Item name="isIncome" label="Direction">
                            <Radio.Group>
                                <Radio value={true}>Income (+)</Radio>
                                <Radio value={false}>Expense (-)</Radio>
                            </Radio.Group>
                        </Form.Item>
                    </div>
                )}

                {type === 'bank' && (
                    <>
                        <div className="grid grid-cols-2 gap-4">
                            <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                                <InputNumber min={0.01} className="w-full" />
                            </Form.Item>
                            <Form.Item name="bankId" label="Bank" rules={[{ required: true }]}>
                                <Select showSearch optionFilterProp="children">
                                    {banks?.map(b => <Option key={b.id} value={b.id}>{b.name}</Option>)}
                                </Select>
                            </Form.Item>
                        </div>
                        <Form.Item name="accountNumber" label="Account Number" rules={[{ required: true }]}>
                            <Input />
                        </Form.Item>
                        <div className="grid grid-cols-2 gap-4">
                            <Form.Item name="interestPercentage" label="Interest % (Annual)">
                                <InputNumber min={0} step={0.1} className="w-full" addonAfter="%" />
                            </Form.Item>
                            <Form.Item name="interestPeriod" label="Period Type (Days/Months)">
                                <Select>
                                    <Option value={0}>Daily</Option>
                                    <Option value={1}>Monthly</Option>
                                    <Option value={2}>Yearly</Option>
                                </Select>
                            </Form.Item>
                        </div>
                    </>
                )}

                {type === 'exchange' && (
                    <>
                        <div className="grid grid-cols-2 gap-4 border p-4 rounded-md bg-gray-50 mb-4">
                            <div>
                                <h4 className="font-semibold mb-2">From Source</h4>
                                <Form.Item name="amount" label="Amount" rules={[{ required: true }]} className="mb-2">
                                    <InputNumber min={0.01} className="w-full" />
                                </Form.Item>
                                {/* Source Currency uses base currencyId already defined above */}
                            </div>
                            <div>
                                <h4 className="font-semibold mb-2">To Target</h4>
                                <Form.Item name="targetAmount" label="Target Amount" rules={[{ required: true }]} className="mb-2">
                                    <InputNumber min={0.01} className="w-full" />
                                </Form.Item>
                                <Form.Item name="targetCurrencyId" label="Target Currency" rules={[{ required: true }]} className="mb-0">
                                    <Select showSearch optionFilterProp="children">
                                        {currencies?.map(c => <Option key={c.id} value={c.id}>{c.code}</Option>)}
                                    </Select>
                                </Form.Item>
                            </div>
                        </div>
                        <Form.Item name="exchangeRate" label="Exchange Rate (Source -> Target)" rules={[{ required: true }]}>
                            <InputNumber min={0.000001} step={0.01} className="w-full" />
                        </Form.Item>
                    </>
                )}

                {type === 'transfer' && (
                    <div className="grid grid-cols-2 gap-4">
                        <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                            <InputNumber min={0.01} className="w-full" />
                        </Form.Item>
                        <Form.Item name="counterpartyId" label="Counterparty" rules={[{ required: true }]}>
                            <Select showSearch optionFilterProp="children">
                                {counterparties?.map(c => <Option key={c.id} value={c.id}>{c.name}</Option>)}
                            </Select>
                        </Form.Item>
                        <Form.Item name="transferType" label="Transfer Type" rules={[{ required: true }]}>
                            <Select>
                                <Option value={0}>Lend (Give)</Option>
                                <Option value={1}>Borrow (Receive)</Option>
                                <Option value={2}>Repay (Give back)</Option>
                                <Option value={3}>Collect (Receive back)</Option>
                            </Select>
                        </Form.Item>
                    </div>
                )}

                {/* Common Fields */}
                <Form.Item name="description" label="Description">
                    <TextArea rows={2} />
                </Form.Item>
                <Form.Item name="tags" label="Tags (Comma separated)">
                    <Input placeholder="e.g. food, salary, vacation" />
                </Form.Item>

            </Form>
        </Modal>
    );
};

export default TransactionModal;
