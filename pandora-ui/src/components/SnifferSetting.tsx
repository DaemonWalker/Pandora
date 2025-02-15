import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Form, Input, Button, Spin, message } from 'antd';

interface DynamicFormProps {
    source: string;
}

export const SnifferSetting: React.FC<DynamicFormProps> = ({ source }) => {
    const [snifferSettings, setSnifferSettings] = useState<Record<string, string>>();
    const [loading, setLoading] = useState(false);
    const [form] = Form.useForm();
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        setLoading(true);
        fetch(`api/sniffer/GetAllConfiguration/${source}`).then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch form items');
            }
            return response;
        }).then(res => res.json())
            .then(json => {
                const config: Record<string, string> = json;
                fetch(`api/sniffer/GetAllKeys/${source}`).then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to fetch form items');
                    }
                    return response;
                }).then(res => res.json())
                    .then(keys => {
                        for (const key of keys) {
                            if (!config[key]) {
                                config[key] = '';
                            }
                        }
                        setSnifferSettings(config);
                    })
            }).finally(() => setLoading(false));
    }, [source]);

    var keys = useMemo(() => snifferSettings ? Object.keys(snifferSettings) : [], [snifferSettings])
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
    if (loading) {
        return <Spin></Spin>
    }

    return (
        snifferSettings ?
            <Form form={form} name="dynamic_form" onFinish={onFinish}>
                {keys.map((item, index) => (
                    <Form.Item labelCol={{ flex: "100px" }} wrapperCol={{ flex: 1 }}
                        key={index}
                        label={item}
                        name={item}
                        initialValue={snifferSettings[item]}
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
