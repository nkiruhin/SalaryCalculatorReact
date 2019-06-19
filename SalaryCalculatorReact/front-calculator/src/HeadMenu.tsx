import React from 'react';
import { createTheme } from '@uifabric/styling';
import { Link } from 'react-router-dom';
import { IButtonProps, CommandBarButton, CommandBar, Persona, PersonaSize, PersonaPresence } from 'office-ui-fabric-react';
import ReactDOM from 'react-dom';
import { Login } from './Auth/Login';

const headerTheme = createTheme({
    palette: {
        themePrimary: '#c7c7c7',
        themeLighterAlt: '#080808',
        themeLighter: '#202020',
        themeLight: '#3c3c3c',
        themeTertiary: '#787878',
        themeSecondary: '#b0b0b0',
        themeDarkAlt: '#cdcdcd',
        themeDark: '#d5d5d5',
        themeDarker: '#e0e0e0',
        neutralLighterAlt: '#0b7cd3',
        neutralLighter: '#1381d5',
        neutralLight: '#2189d8',
        neutralQuaternaryAlt: '#2a8dda',
        neutralQuaternary: '#3191db',
        neutralTertiaryAlt: '#50a2e1',
        neutralTertiary: '#fffefd',
        neutralSecondary: '#fffefd',
        neutralPrimaryAlt: '#fffefe',
        neutralPrimary: '#fffefc',
        neutralDark: '#fffffe',
        black: '#ffffff',
        white: '#0378d1',
    }
});
const _addLink = (props: IButtonProps) => (
    <Link to={props.data !== undefined ? props.data : '/'}>
        <CommandBarButton {...props} theme={headerTheme} />
    </Link >
)

const _logout = () => {
    localStorage.clear();
    ReactDOM.render(<Login />, document.getElementById('root'))
}

const asPersona = (props: IButtonProps) => (
    <div style={{ padding: '8px' }}>
        <Persona text={localStorage.name !== null ? localStorage.name : ""}
            size={PersonaSize.size28}
            theme={headerTheme}
            presence={PersonaPresence.online}
        />
    </div>
)
const _commandBarItems = [
    {
        key: 'employes', name: 'Сотрудники', ariaLabel: 'Сотрудники',
        data: '/',
        commandBarButtonAs: _addLink
    },
    {
        key: 'position', name: 'Должности', ariaLabel: 'Должности',
        data: 'positions',
        commandBarButtonAs: _addLink
    },
    {
        key: 'salaryreports', name: 'Отчет по зарплате', ariaLabel: 'Отчет по зарплате',
        data: 'salaryreports',
        commandBarButtonAs: _addLink
    }
]
const _farItems = [
    {
        key: 'user',
        commandBarButtonAs: asPersona
    },
    {
        key: 'exit',
        name: 'Выход',
        ariaLabel: 'Выход',
        theme: headerTheme,
        iconProps: {
            iconName: 'Leave',
        },
        iconOnly: true,
        onClick: () => _logout()
    }
]


export const HeadMenu: React.FC = () => {
    if (localStorage.Role !== "Administrator") {
        _commandBarItems.splice(1,1)
    }
    return <CommandBar
        items={_commandBarItems}
        styles={() => ({ root: { height:'3.5em' } })}
        theme={headerTheme}
        farItems={_farItems}
        ariaLabel={'Use left and right arrow keys to navigate between commands'}
    />
}