import { FC } from 'react';
import { Tabs } from 'antd';
import { useAutoFetchTabKeys } from '../store/allSourceStore';
import { SnifferSetting } from '../components/SnifferSetting';

const { TabPane } = Tabs;

export const Settings: FC = () => {
    const { tabKeys, isLoading, error } = useAutoFetchTabKeys();

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <Tabs defaultActiveKey="1">
            {tabKeys.map((key, index) => (
                <TabPane tab={key} key={index + 1}>
                    <SnifferSetting source={key} />
                </TabPane>
            ))}
        </Tabs>
    );
};
