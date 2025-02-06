import { useEffect, FC } from 'react';
import { Tabs } from 'antd';
import { useSettingsStore } from '../store/settingsStore';
import { SinfferSetting } from '../components/SinfferSetting';

const { TabPane } = Tabs;

export const Settings: FC = () => {
    const { tabKeys, isLoading, error, fetchTabKeys } = useSettingsStore();

    useEffect(() => {
        fetchTabKeys();
    }, [fetchTabKeys]);

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
                    <SinfferSetting source={key} />
                </TabPane>
            ))}
        </Tabs>
    );
};
