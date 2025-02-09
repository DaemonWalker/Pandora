import React, { useState } from 'react';
import {
  DesktopOutlined,
  FileOutlined,
  HomeOutlined,
  PieChartOutlined,
  SettingOutlined,
  TeamOutlined,
  UserOutlined,
} from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { Breadcrumb, Layout, Menu, theme } from 'antd';
import { NavLink, Route, Routes, useLocation } from 'react-router';
import { Search } from './pages/Search';
import { Settings } from './pages/Settings';

const { Header, Content, Footer, Sider } = Layout;

type MenuItem = Required<MenuProps>['items'][number] & { link?: string };

function getItem(
  label: React.ReactNode,
  key: React.Key,
  icon?: React.ReactNode,
  link?: string
): MenuItem {
  return {
    key,
    icon,
    label: link ? <NavLink to={link}>{label}</NavLink> : label,
    link
  } as MenuItem;
}

const items: MenuItem[] = [
  getItem('主页', '1', <HomeOutlined />, "/"),
  getItem('设置', '2', <SettingOutlined />, "/settings")
];

const App: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false);
  const {
    token: { colorBgContainer, borderRadiusLG, colorBgLayout },
  } = theme.useToken();
  const location = useLocation();

  // 根据当前 URL 找到对应的菜单项 key
  const selectedKey = (items.find(item => item?.link === location.pathname)?.key || '1').toString();

  return (
    <Layout style={{ height: '100vh' }}>
      <Sider collapsible collapsed={collapsed} onCollapse={(value) => setCollapsed(value)}>
        <div className="demo-logo-vertical" />
        <Menu theme="dark" selectedKeys={[selectedKey]} defaultSelectedKeys={['1']} mode="inline" items={items} />
      </Sider>
      <Layout>
        <Header style={{ padding: 0, background: colorBgLayout }} />
        <Content style={{ margin: '0 16px', flex: 1 }}>
          <div
            style={{
              padding: 24,
              minHeight: 360,
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
              maxHeight: "100%",
              overflow: "auto",
            }}
          >
            <Routes>
              <Route index element={<Search />}></Route>
              <Route path='/settings' element={<Settings />}></Route>
            </Routes>
          </div>
        </Content>
        <Footer style={{ textAlign: 'center' }}>
          Ant Design ©{new Date().getFullYear()} Created by Ant UED
        </Footer>
      </Layout>
    </Layout>
  );
};

export default App;