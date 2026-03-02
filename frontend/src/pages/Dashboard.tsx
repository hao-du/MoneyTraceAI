import React from 'react';
import { Card, Table, Typography, Tag, DatePicker, Statistic, Row, Col } from 'antd';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import api from '../api/axios';
import { useTranslation } from 'react-i18next';
import { TransactionOutlined, ArrowUpOutlined, ArrowDownOutlined } from '@ant-design/icons';

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

const Dashboard: React.FC = () => {
    const { t } = useTranslation();
    const [dates, setDates] = React.useState<[dayjs.Dayjs, dayjs.Dayjs] | null>(null);

    const { data: transactions, isLoading } = useQuery({
        queryKey: ['dashboard_transactions', dates],
        queryFn: async () => {
            let url = '/dashboard/transactions';
            if (dates && dates[0] && dates[1]) {
                url += `?startDateUtc=${dates[0].toISOString()}&endDateUtc=${dates[1].toISOString()}`;
            }
            const response = await api.get<TransactionDto[]>(url);
            return response.data;
        },
    });

    const incomeTotal = transactions?.filter(t => t.isIncome).reduce((sum, current) => sum + current.amount, 0) || 0;
    const expenseTotal = transactions?.filter(t => !t.isIncome).reduce((sum, current) => sum + current.amount, 0) || 0;

    const columns = [
        {
            title: 'Date',
            dataIndex: 'dateUtc',
            key: 'dateUtc',
            render: (text: string) => dayjs(text).format('YYYY-MM-DD HH:mm'),
        },
        {
            title: 'Type',
            dataIndex: 'type',
            key: 'type',
            render: (type: string) => <Tag color="blue">{type}</Tag>,
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
        }
    ];

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center mb-6">
                <Title level={3} className="!m-0">{t('dashboard')}</Title>
                <RangePicker
                    onChange={(vals) => setDates(vals as [dayjs.Dayjs, dayjs.Dayjs] | null)}
                    allowClear
                />
            </div>

            <Row gutter={[16, 16]}>
                <Col xs={24} sm={12} md={8}>
                    <Card bordered={false} className="shadow-sm">
                        <Statistic
                            title="Total Income"
                            value={incomeTotal}
                            precision={2}
                            valueStyle={{ color: '#10b981' }}
                            prefix={<ArrowUpOutlined />}
                        />
                    </Card>
                </Col>
                <Col xs={24} sm={12} md={8}>
                    <Card bordered={false} className="shadow-sm">
                        <Statistic
                            title="Total Expense"
                            value={expenseTotal}
                            precision={2}
                            valueStyle={{ color: '#ef4444' }}
                            prefix={<ArrowDownOutlined />}
                        />
                    </Card>
                </Col>
                <Col xs={24} sm={12} md={8}>
                    <Card bordered={false} className="shadow-sm">
                        <Statistic
                            title="Net Balance"
                            value={incomeTotal - expenseTotal}
                            precision={2}
                            prefix={<TransactionOutlined />}
                        />
                    </Card>
                </Col>
            </Row>

            <Card title="Recent Transactions" bordered={false} className="shadow-sm">
                <Table
                    columns={columns}
                    dataSource={transactions}
                    rowKey="id"
                    loading={isLoading}
                    pagination={{ pageSize: 10 }}
                />
            </Card>
        </div>
    );
};

export default Dashboard;
