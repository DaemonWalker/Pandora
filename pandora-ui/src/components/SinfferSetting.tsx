import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Spin, message } from 'antd';
import { useSnifferSettingStore } from '../store/snifferSettingStore';

interface DynamicFormProps {
    source: string;
}

export const SinfferSetting: React.FC<DynamicFormProps> = ({ source }) => {
    const { snifferSettings, isLoading, error, fetchFormItems } = useSnifferSettingStore();
    const [form] = Form.useForm();
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        fetchFormItems(source);
    }, [source, fetchFormItems]);

    const snifferSetting = snifferSettings[source] || [];
    const loading = isLoading[source] || false;
    const err = error[source];
    const onFinish = (values: any) => {
        console.log('Form values:', values);
    };

    const save = async () => {
        setSaving(true);
        const formValues = form.getFieldsValue();
        const jsonData: any = {};
        snifferSetting.forEach((label) => {
            if (formValues[label] !== undefined) {
                jsonData[label] = (formValues[label] ?? "").trim();
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

    if (loading) {
        return <Spin />;
    }
    console.log(err)

    if (err) {
        return <div>Error: {err}</div>;
    }

    return (
        <Form form={form} name="dynamic_form" onFinish={onFinish}>
            {snifferSetting.map((item, index) => (
                <Form.Item labelCol={{ flex: "100px" }} wrapperCol={{ flex: 1 }}
                    key={index}
                    label={item}
                    name={item}
                >
                    <Input.TextArea />
                </Form.Item>
            ))}
            <Form.Item>
                <Button type="default" onClick={save} loading={saving}>
                    保存
                </Button>
            </Form.Item>
        </Form >
    );
};
