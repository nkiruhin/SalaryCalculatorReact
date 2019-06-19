import React from "react";
import { Breadcrumb } from 'office-ui-fabric-react/lib/Breadcrumb'
import { CommandBar } from 'office-ui-fabric-react/lib/CommandBar'
import Dialog from "office-ui-fabric-react/lib/Dialog";
import fetchApi from "../fetcher";
import { MessageBar, MessageBarType } from "office-ui-fabric-react";
import PositionsTable from "./PositionTable";
import { Form } from "../Main/Form";
import { DeleteDialog } from "../Main/DeleteDialog";



export const Positions: React.FC = () => {
    const [hideDialog, setHideDialog] = React.useState(true)
    const [deleteDialog, setDeleteDialog] = React.useState(true)
    const [message, setMessage] = React.useState();
    const [Id, setId] = React.useState(0)
    const [titleDialog, setTitleDialog] = React.useState("Новая должность")
    const [refresh, setRefresh] = React.useState(true);
    const _add = (): void => {
        setTitleDialog("Новая должность")
        setId(0);
        setHideDialog(false);        
    };
    const _edit = (id: number): void => {
        setTitleDialog("Редактирование")
        setHideDialog(false);
        setId(id);
    };
    const _delete = (id: number) => {
        setDeleteDialog(false)
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
    const isTrue = (answer: boolean) => {
        if (answer) {
            let url = "api/Position/" + Id
            fetchApi(returnData, url, "DELETE")
        }
        setDeleteDialog(true)
        
    };
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
            key: 'addItem', name: 'Добавить', ariaLabel: 'Добавить', onClick: () => _add(),
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
        var items = [
            {
                key: 'addItem', name: 'Добавить', ariaLabel: 'Добавить', onClick: () => _add(),
                iconProps: {
                    iconName: 'Add'
                },
            },
            {
                key: 'editItem', name: 'Редактировать', ariaLabel: 'Редактировать', onClick: () => _edit(id),
                iconProps: {
                    iconName: 'Edit'
                }
            },
            {
                key: 'deleteItem', name: 'Удалить', ariaLabel: 'Удалить', onClick: () => _delete(id),
                iconProps: {
                    iconName: 'Delete'
                }
            }
        ]
        setcommandBarItems(items);
        setId(id)
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
    //const _closeDialog = () => {
    //    setHideDialog(true)
    //}
    return (
        <div>
            <Breadcrumb className="breadcrumbs" items={[
                { text: 'Должности', key: 'Positions' },
                { text: 'Список', key: 'PositionsList' }
            ]}
                    maxDisplayedItems={3}
            />
            <CommandBar
                items={ commandBarItems }
                farItems={ _farItems }
                ariaLabel={'Use left and right arrow keys to navigate between commands'}
            />
            {message}
            
            <PositionsTable isEdit={isSelect} refresh = {refresh} setRefresh = {setRefresh} />
            <Dialog hidden={hideDialog}
                onDismiss={()=>_hideDialog(true,false)}
                title={ titleDialog }
                minWidth={"1100px"}
            >
                <Form name={"Position"} id={Id} setHideDialog={_hideDialog} edit={true} />
            </Dialog>
            <DeleteDialog hidden={deleteDialog} istrue={isTrue} title="Удаление должности" text="Удалить должность?" />
        </div>        
        )
}

export default Positions