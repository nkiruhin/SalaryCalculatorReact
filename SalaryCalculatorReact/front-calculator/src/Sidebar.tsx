import { getTheme } from 'office-ui-fabric-react';
import * as React from 'react';
import { Sidebar, SidebarButton, ISidebarItemProps } from '@uifabric/experiments/lib/Sidebar';
import { Link } from 'react-router-dom';



export const AppSidebar: React.FC = () => {

    const [active, setActive] = React.useState("/");
    const items: ISidebarItemProps[] = [
        {
            key: '/',
            name: 'Сотрудники',
            iconProps: { iconName: 'People' },
            active: true,
            onRender: item => (_onRenderLink(item))
        },
        {
            key: 'positions',
            name: 'Должности',
            iconProps: { iconName: 'PartyLeader' },
            active: false,
            onRender: item => (_onRenderLink(item))
        },
        {
            key: 'salaryreports',
            name: 'Зарплата',
            iconProps: { iconName: 'Money' },
            active: false,
            onRender: item => (_onRenderLink(item))
        }
    ]
    const _onRenderLink = (item: any) => (

        <Link to={item.key} key={item.key}>
            <SidebarButton
                key={item.key}
                text={item.name}
                iconProps={item.iconProps}
                role="menuitem" theme={getTheme()}
                checked={item.key === active ? true : false}
                onClick={_onLinkClick}
                id={item.key}
            />
        </Link>
    )
    const _onLinkClick = (ev: React.MouseEvent<any, MouseEvent>) => {
        setActive(ev.currentTarget.id)
    }
    if (localStorage.Role !== "Administrator") {
       items.splice(1, 1)
    }
    return <Sidebar
        collapsible={true}
        collapseButtonAriaLabel={'sitemap'}
        theme={getTheme()}
        items={items}>
    </Sidebar>
}
