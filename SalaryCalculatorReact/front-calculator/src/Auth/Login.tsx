import React from 'react';
import { Fabric, TextField,  Stack, PrimaryButton } from 'office-ui-fabric-react';
import { Depths } from '@uifabric/fluent-theme/lib/fluent/FluentDepths';
import fetchApi from '../fetcher';
import ReactDOM from 'react-dom';
import App from '../App';

const _onGetErrorMessage = (value: string) => (
    value === '' ? "Введите данные" : ''
)



export const Login: React.FC = () => {
    const [errorMessage, setErrorMessage] = React.useState("")
    const [username, setUsername] = React.useState("")
    const [password, setPassword] = React.useState("")
    const _response = (data: any, status: number) => {
        if (status !== 200) {
            setErrorMessage(data.error)
        } else {
            localStorage.setItem("Token", data.token)
            localStorage.setItem("Role", data.role)
            localStorage.setItem("name", data.name)
            ReactDOM.render(<App />, document.getElementById('root'));
        }
    }
    const _enter = () => {
        if (!username || !password) {
            return
        }
        let url = "api/Auth/token?username=" + username + "&password=" + password;
        fetchApi(_response,url,"GET")
    }
    return <Fabric>
        <Stack horizontalAlign="center" tokens={{ maxWidth: 'auto'}}>
            <div style={{ boxShadow: Depths.depth8, marginTop: "120px", padding: "30px 80px 50px 80px" }}>
                <h2>Вход в систему</h2>
                {errorMessage !== "" ? <span style={{ color:"red" }}> {errorMessage} </span> : ""}
                <TextField
                    label="Имя пользователя"
                    required={true}
                    onChange={(evn: any, value: any) => (setUsername(value))}
                    onGetErrorMessage={_onGetErrorMessage}
                    value={username}
                />
                <TextField
                    label="Пароль"
                    type={"password"}
                    required={true}
                    onGetErrorMessage={_onGetErrorMessage}
                    onChange={(evn: any, value: any) => (setPassword(value))}
                    value={password}
                />
                <p />
                <PrimaryButton text="Войти" onClick={ _enter } />
            </div>
        </Stack>
    </Fabric>
};