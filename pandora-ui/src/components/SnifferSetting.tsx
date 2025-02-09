import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Form, Input, Button, Spin, message } from 'antd';
import { useSnifferSettingStore } from '../store/snifferSettingStore';

interface DynamicFormProps {
    source: string;
}

export const SnifferSetting: React.FC<DynamicFormProps> = ({ source }) => {
    const { snifferSettings, fetchFormItems } = useSnifferSettingStore();
    const [form] = Form.useForm();
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        fetchFormItems(source)
    }, [source]);

    var config = useMemo(() => snifferSettings ? snifferSettings[source] : null, [snifferSettings])
    var keys = useMemo(() => config ? Object.keys(config) : [], [config])

    const snifferSetting = snifferSettings[source] || [];
    const onFinish = (values: any) => {
        console.log('Form values:', values);
    };

    const save = async () => {
        setSaving(true);
        const formValues = form.getFieldsValue();
        const jsonData: any = {};
        keys.forEach((label) => {
            const value = formValues[label]?.trim();
            if (value && value.length > 0) {
                jsonData[label] = value;
            }
        });
        try {
            const response = await fetch(`/api/Sniffer/SetConfiguration/${source}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(jsonData)
            });
            if (!response.ok) {
                throw new Error('Failed to save configuration');
            }
            message.success('更新成功');
        } catch (error) {
            console.error('Error saving configuration:', error);
            message.error('更新失败');
        }
        finally {
            setSaving(false);
        }
    }

    return (
        config ?
            <Form form={form} name="dynamic_form" onFinish={onFinish}>
                {keys.map((item, index) => (
                    <Form.Item labelCol={{ flex: "100px" }} wrapperCol={{ flex: 1 }}
                        key={index}
                        label={item}
                        name={item}
                        initialValue={snifferSetting[item]}
                    >
                        <Input.TextArea autoSize />
                    </Form.Item>
                ))
                }
                <Form.Item>
                    <Button type="default" onClick={save} loading={saving}>
                        保存
                    </Button>
                </Form.Item>
            </Form >
            : <>loading...</>
    );
};
