import React from "react";
import { Breadcrumb } from 'office-ui-fabric-react/lib/Breadcrumb'
import { CommandBar } from 'office-ui-fabric-react/lib/CommandBar'
import EmployesTable from './EmployesTable'
import Dialog from "office-ui-fabric-react/lib/Dialog";
import fetchApi from "../fetcher";
import { MessageBar, MessageBarType } from "office-ui-fabric-react";
import { DeleteDialog, IDeleteDialogProps } from "../Main/DeleteDialog";
import { Form } from "../Main/Form";




export const Employees: React.FC = () => {

    const isTrue = (answer: boolean) => {
        if (answer) {
            console.log(formName)
            let url = "api/" + formName + "/" + employeeId
            fetchApi(returnData, url, "DELETE")
        }
        _deleteDialog.hidden = true
        setDeleteDialog(_deleteDialog)
    };
    const _deleteDialog: IDeleteDialogProps = {
        hidden: true,
        text: '',
        title: '',
        istrue: isTrue
    }
    const [hideDialog, setHideDialog] = React.useState(true)
    const [deleteDialog, setDeleteDialog] = React.useState(_deleteDialog)
    const [message, setMessage] = React.useState();
    const [employeeId, setEmployeeId] = React.useState(0)
    const [titleDialog, setTitleDialog] = React.useState("Новый сотрудник")
    const [refresh, setRefresh] = React.useState(true);
    const [editForm, setEditForm] = React.useState(true);
    const [formName, setFormName] = React.useState("Employees");
    const _addEmployee = (): void => {
        setFormName("Employees");
        setTitleDialog("Новый сотрудник")
        setEmployeeId(0);
        setHideDialog(false);        
    };
    const _editEmployee = (id: number): void => {
        if (localStorage.Role !== "Administrator") {
            setTitleDialog("Просмотр")
            setEditForm(false)
        } else {
            setTitleDialog("Редактирование")
        }       
        setHideDialog(false);
        setFormName("Employees");
        setEmployeeId(id);
    };
    const _deleteEmployee = () => {      
        setFormName("Employees");
        _deleteDialog.hidden = false
        _deleteDialog.text = 'Вы уверены что хотите удалите сотрудника?'
        _deleteDialog.title = 'Удаление сотрудника'
        setDeleteDialog(_deleteDialog)
    };
    const _accountManager = (event: string) => {
        if (event === "delete") {
            setFormName("Auth");
            _deleteDialog.hidden = false
            _deleteDialog.text = 'Вы уверены что хотите удалите учетную запись сотрудника?'
            _deleteDialog.title = 'Удаление учетной записи сотрудника'
            setDeleteDialog(_deleteDialog)
            return;
        }
        setFormName("Auth");
        setTitleDialog("Учетные данные");
        setHideDialog(false);
    };
    const _hideDialog = (hide: boolean, submit?: boolean, _message?: string): boolean => {
        setHideDialog(hide)
        if (_message !== undefined) {
            setMessage(
                <MessageBar messageBarType={ MessageBarType.success } isMultiline={false} onDismiss={() => setMessage(null)} dismissButtonAriaLabel="Close">
                    {_message}
                </MessageBar>
            )
        }
        if (submit) setRefresh(true)
        return hideDialog
    }
    
    const returnData = (data: any, status: any) => {
        let _messageType
        let _message

        if (status === 400) {
            _messageType = MessageBarType.error;
            _message = data.errors;

        } else {
            _messageType = MessageBarType.success;
            _message = data.message;
            setRefresh(true)
        }
        setMessage(
            <MessageBar messageBarType={_messageType} isMultiline={false} onDismiss={() => setMessage(null)} dismissButtonAriaLabel="Close">
                {_message}
            </MessageBar>
        )
    }
    

    const _commandBarItems =  [
        {
            key: 'addItem', name: 'Добавить', ariaLabel: 'Добавить', onClick: () => _addEmployee(),
            iconProps: {
                iconName: 'Add'
            },
        },
        {
            key: 'refresh', name: 'Обновить', ariaLabel: 'Обновить', onClick: () => setRefresh(true),
            iconProps: {
                iconName: 'Refresh'
            },
        }
    ]
    const [commandBarItems, setcommandBarItems] = React.useState(_commandBarItems)
    const isSelect = (is: boolean, id: number): void => {
        
        if (!is) {
            setcommandBarItems(_commandBarItems);
            return;
        }
        if (localStorage.Role !== "Administrator") {
            var items = [
                {
                    key: 'Info', name: 'Просмотреть', ariaLabel: 'Просмотреть', onClick: () => _editEmployee(id),
                    iconProps: {
                        iconName: 'info'
                    },
                },
            ]
        } else {
        items = [
            {
                key: 'addItem', name: 'Добавить', ariaLabel: 'Добавить', onClick: () => _addEmployee(),
                iconProps: {
                    iconName: 'Add'
                },
            },
            {
                key: 'editItem', name: 'Редактировать', ariaLabel: 'Редактировать', onClick: () => _editEmployee(id),
                iconProps: {
                    iconName: 'Edit'
                }
            },
            {
                key: 'deleteItem', name: 'Удалить', ariaLabel: 'Удалить', onClick: () => _deleteEmployee(),
                iconProps: {
                    iconName: 'Delete'
                }
            }
            ]
        }
        setcommandBarItems(items);
        setEmployeeId(id)
    }
    const _farItems = [
            {
                key: 'sort',
                name: 'Sort',
                ariaLabel: 'Sort',
                iconProps: {
                    iconName: 'SortLines'
                },
                onClick: () => console.log('Sort')
            },
            {
                key: 'tile',
                name: 'Grid view',
                ariaLabel: 'Grid view',
                iconProps: {
                    iconName: 'Tiles'
                },
                iconOnly: true,
                onClick: () => console.log('Tiles')
            },
            {
                key: 'info',
                name: 'Info',
                ariaLabel: 'Info',
                iconProps: {
                    iconName: 'Info'
                },
                iconOnly: true,
                onClick: () => console.log('Info')
            }
    ]
   
    return (
        <div>
            <Breadcrumb className="breadcrumbs" items={[
                { text: 'Сотрудники', key: 'Employees' },
                { text: 'Список', key: 'EmployeesList' }
            ]}
                    maxDisplayedItems={3}
            />
            <CommandBar
                items={ commandBarItems }
                farItems={ _farItems }
                ariaLabel={'Use left and right arrow keys to navigate between commands'}
            />
            {message}

            <EmployesTable isEdit={isSelect} refresh={refresh} setRefresh={setRefresh} setAccount={_accountManager} />
            <Dialog hidden={hideDialog}
                onDismiss={()=>_hideDialog(true,false)}
                title={ titleDialog }
                minWidth={"1100px"}
            >
                <Form name={ formName } id={employeeId} setHideDialog={_hideDialog} edit={editForm} />
            </Dialog>
            <DeleteDialog {...deleteDialog} istrue={isTrue} />
        </div>        
        )
}

export default Employees