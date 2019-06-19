import * as React from 'react';
import { Stack } from 'office-ui-fabric-react/lib/Stack';
import { IComboBoxOption } from 'office-ui-fabric-react/lib/ComboBox'
import { DialogFooter } from 'office-ui-fabric-react/lib/Dialog'
import { PrimaryButton, DefaultButton } from 'office-ui-fabric-react/lib/Button';
import fetchApi from '../fetcher';
import { Spinner } from 'office-ui-fabric-react';
import { Field } from '../Main/Fields';

export interface IFormField {
    name: string;
    displayName: string;
    type: string;
    isRequired: boolean;
    items: IComboBoxOption[];
    serverSide: boolean;
    value: string;
}

var formData = new FormData();

interface IEmployeeForm {
    name: string
    id: number
    setHideDialog(hide: boolean, submit?: boolean, message?: string): void
    edit: boolean
}
const noerror: any = {}
export const Form: React.FC<IEmployeeForm> = ({ name, id, setHideDialog, edit }) => {
    const initFields: IFormField[] = [];   
    const [fields, setFields] = React.useState(initFields)
    const [response, setResponse] = React.useState(noerror)
    const [spinner, setSpinner] = React.useState()
    const _showSpinner = (show: boolean) => {
        if (show) {
            setSpinner(
                <Spinner label="Подождите идет загрузка..." ariaLive="assertive" labelPosition="right" />
            )
        } else {
            setSpinner('')
        }
    }
    const _fields = React.useCallback((data: any, status: number) => {
        if (status === 200) {
            formData = new FormData()
            setFields(data)
        } else {
            setResponse(data.errors);
        }
        _showSpinner(false)
    },[]
    )
   
    React.useEffect(() => {
        _showSpinner(true)
        let url = "api/" + name + "/Form/" + id
        fetchApi(_fields, url, "GET")
    }, [id, name, _fields])
    
    const _setResponse = (data: any) => {
       
        if (data.errors) {
            setResponse(data.errors);
        } else {
            setHideDialog(true, true, data.message)
        }
        _showSpinner(false)
    }

    const onSubmit = (evn: any) => {        
        formData.forEach((value, key) => {
            console.log(key + " " + value)
        });
        let metod: string = id > 0 ? "PUT" : "POST";
        _showSpinner(true);
        fetchApi(_setResponse, "api/" + name +"/" + (id === 0 ? "" : id.toString()), metod, formData)
        evn.preventDefault();
    }
    const onChange = (name:string, value: any) => {
        formData.set(name, value);

    }
    const _hideDialog = () => {
        setHideDialog(true, false)
    }
   
    return (   
    <form onSubmit={onSubmit}>
            {Object.keys(response).map((key) => (<div style={{ color: "red" }} key={key}>{response[key]}</div>))}           
            {spinner}
            <Stack horizontal={true} tokens={{ padding: '10px', childrenGap:'10px' }} wrap={true} horizontalAlign={"baseline"} >
                {fields.map((field: IFormField, index: number) => {
                    formData.set(field.name, field.value === null ? '' : field.value)
                    return <Field key={field.name}
                        id={index}
                        type={field.type}
                        name={field.name}
                        label={field.displayName}
                        items={field.items}
                        serverSide={field.serverSide}
                        formName={name}
                        required={field.isRequired}
                        value={field.value === null ? '' : field.value}
                        onChange={onChange}
                        edit={edit}
                    />
                })}
            </Stack>
            <DialogFooter>
                {edit?<PrimaryButton type="Submit" text="Сохранить" />:''}
                <DefaultButton text="Отмена" onClick={_hideDialog} />
            </DialogFooter>
        </form >
    )
}