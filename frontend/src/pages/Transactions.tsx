import React, { useState } from 'react';
import { Card, Table, Typography, Tag, DatePicker, Button, Space } from 'antd';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import api from '../api/axios';
import { useTranslation } from 'react-i18next';
import { PlusOutlined } from '@ant-design/icons';
import TransactionModal from '../components/organisms/TransactionModal';

const { Title } = Typography;
const { RangePicker } = DatePicker;

interface TransactionDto {
    id: string;
    dateUtc: string;
    amount: number;
    currencyId: string;
    description?: string;
    tags?: string;
    type: string;
    isIncome: boolean;
}

const Transactions: React.FC = () => {
    const { t } = useTranslation();
    const [dates, setDates] = useState<[dayjs.Dayjs, dayjs.Dayjs] | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const { data: transactions, isLoading } = useQuery({
        queryKey: ['transactions_list', dates],
        queryFn: async () => {
            let url = '/dashboard/transactions'; // Re-using dashboard unified endpoint for list
            if (dates && dates[0] && dates[1]) {
                url += `?startDateUtc=${dates[0].toISOString()}&endDateUtc=${dates[1].toISOString()}`;
            }
            const response = await api.get<TransactionDto[]>(url);
            return response.data;
        },
    });

    const columns = [
        {
            title: 'Date',
            dataIndex: 'dateUtc',
            key: 'dateUtc',
            render: (text: string) => dayjs(text).format('YYYY-MM-DD HH:mm'),
            sorter: (a: TransactionDto, b: TransactionDto) => new Date(a.dateUtc).getTime() - new Date(b.dateUtc).getTime(),
        },
        {
            title: 'Type',
            dataIndex: 'type',
            key: 'type',
            render: (type: string) => <Tag color="blue">{type}</Tag>,
            filters: [
                { text: 'Cashflow', value: 'Cashflow' },
                { text: 'Bank', value: 'Bank' },
                { text: 'Exchange', value: 'Exchange' },
                { text: 'Transfer', value: 'Transfer' },
            ],
            onFilter: (value: boolean | React.Key, record: TransactionDto) => record.type === value,
        },
        {
            title: 'Amount',
            dataIndex: 'amount',
            key: 'amount',
            render: (amount: number, record: TransactionDto) => (
                <span className={record.isIncome ? 'text-success font-semibold' : 'text-danger font-semibold'}>
                    {record.isIncome ? '+' : '-'}{amount.toLocaleString()}
                </span>
            ),
            sorter: (a: TransactionDto, b: TransactionDto) => a.amount - b.amount,
        },
        {
            title: 'Description',
            dataIndex: 'description',
            key: 'description',
        },
        {
            title: 'Tags',
            dataIndex: 'tags',
            key: 'tags',
            render: (tags: string) => tags ? tags.split(',').map(tag => <Tag key={tag}>{tag.trim()}</Tag>) : null,
        },
        {
            title: 'Actions',
            key: 'actions',
            render: () => (
                <Space>
                    {/* Details / Edit links would go here */}
                    <Button type="link" size="small">Details</Button>
                </Space>
            )
        }
    ];

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
                <Title level={3} className="!m-0">{t('transactions')}</Title>
                <Space>
                    <RangePicker
                        onChange={(vals) => setDates(vals as [dayjs.Dayjs, dayjs.Dayjs] | null)}
                        allowClear
                    />
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => setIsModalOpen(true)}>New Transaction</Button>
                </Space>
            </div>

            <Card bordered={false} className="shadow-sm">
                <Table
                    columns={columns}
                    dataSource={transactions}
                    rowKey="id"
                    loading={isLoading}
                    pagination={{ pageSize: 20 }}
                    scroll={{ x: 'max-content' }}
                />
            </Card>
            <TransactionModal open={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </div>
    );
};

export default Transactions;
