import * as React from 'react';
import { DetailsList, DetailsListLayoutMode, Selection, SelectionMode, IColumn } from 'office-ui-fabric-react/lib/DetailsList';
import fetchApi from '../fetcher';
import { Persona, PersonaSize, CommandButton } from 'office-ui-fabric-react';



interface IEmployeeItem {
    id: number;
    name: string;
    birthDay: string;
    dateofRecruitment: string;
    position: string;
    manager: string;
    account: boolean;
}

interface IEmployesTable {
    isEdit(is: boolean, id: number): void
    refresh: boolean
    setRefresh(refresh: boolean): void
    setAccount(event: string): void
}

const EmployesTable: React.FC<IEmployesTable> = ({ isEdit, refresh, setRefresh, setAccount }) => {

    const url = "api/Employees";
    const _accountRow = (item: IEmployeeItem) => {
        if (item.account) {
            return <CommandButton
                data-automation-id="account"
                iconProps={{ iconName: 'AccountManagement' }}
                //text="Учетная запись"
                menuProps={{
                    items: [
                        {
                            key: 'delete',
                            text: 'Удалить',
                            iconProps: { iconName: 'Delete' },
                            onClick: () => { setAccount('delete') }
                        },
                        {
                            key: 'changePassword',
                            text: 'Изменить пароль',
                            iconProps: { iconName: 'Edit' },
                            onClick: () => { setAccount('edit') }
                        },
                    ]
                }}
            />

        } else {
            return <CommandButton
                data-automation-id="account"
                iconProps={{ iconName: 'AccountManagement', styles: { root: { color: '#808080' } } }}
                //text="Учетная запись"
                menuProps={{
                    items: [
                        {
                            key: 'addAccount',
                            text: 'Создать',
                            iconProps: {
                                iconName: 'Add'
                            },
                            onClick: () => { setAccount('create') }
                        },

                    ]
                }}
            />
        }
    }
   const columns: IColumn[] = [
            {
                key: 'id',
                name: 'Id',
                fieldName: 'id',
                minWidth: 16,
                maxWidth: 16,
                data: 'number',
            },
            {
                key: 'name',
                name: 'ФИО',
                minWidth: 210,
                maxWidth: 350,
                fieldName: 'name',
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                isPadded: true,
                onRender: (item: IEmployeeItem) => (<Persona text={item.name}  size={PersonaSize.size32} />)
            },
            {
                key: 'BirthDay',
                name: 'Дата рождения',
                fieldName: 'birthDay',
                minWidth: 70,
                maxWidth: 90,
                isResizable: true,
                isCollapsible: true,
                data: 'number',
            },
            {
                key: 'dateofRecruitment',
                name: 'Дата приема',
                fieldName: 'dateofRecruitment',
                minWidth: 70,
                maxWidth: 90,
                isResizable: true,
                isCollapsible: true,
                data: 'string',
            },
            {
                key: 'position',
                name: 'Должность',
                fieldName: 'positionName',
                minWidth: 70,
                maxWidth: 110,
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                isPadded: true
            },
            {
                key: 'manager',
                name: 'Руководитель',
                fieldName: 'managerName',
                minWidth: 210,
                maxWidth: 350,
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                isPadded: true
       },
            {
           key: 'Account',
           name: 'Учетная запись',
           fieldName: 'account',
           minWidth: 210,
           maxWidth: 350,
           isRowHeader: true,
           isResizable: true,
           data: 'string',
           isPadded: true,
           onRender: _accountRow
       },
    ];
   const emptyItems: IEmployeeItem[] = [];
   const _selection = new Selection({
       onSelectionChanged: () => {
           var id = _selection.getSelection()[0] === undefined ? 0 : (_selection.getSelection()[0] as IEmployeeItem).id
           if (id === 0) {
               isEdit(false, 0)
               return;
           }
           isEdit(true, id)
           }
        });
    const [items, setItems] = React.useState(emptyItems)
    const _setItems = (data: IEmployeeItem[], status: number) => {
        if (status === 200) {
            setItems(data)
        } else {
            console.log("Ошибка загрузки данных")
        }
    }
    React.useEffect(() => {
        if(refresh) fetchApi(_setItems, url, "GET")
        setRefresh(false)
    }, [refresh, setRefresh])
    // Убираем для пользователей управление учетками
    if (localStorage.Role !== "Administrator") {
        columns.pop()
    } 
   return (
       <div style={{
           height: '570px',
           position: 'relative',
           maxHeight: '40%',
           overflow: 'auto'
       }}>

           <DetailsList
               items={items}
               columns={columns}
                selectionMode={SelectionMode.single}
                selection={_selection}
                setKey="set"
                layoutMode={DetailsListLayoutMode.justified}
                isHeaderVisible={true}
                selectionPreservedOnEmptyClick={true}
               />
    </div>
   )
}

export default EmployesTable
