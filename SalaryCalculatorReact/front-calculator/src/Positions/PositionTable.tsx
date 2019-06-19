import * as React from 'react';
import { DetailsList, DetailsListLayoutMode, Selection, SelectionMode, IColumn } from 'office-ui-fabric-react/lib/DetailsList';
import fetchApi from '../fetcher';



interface IPositionItem {
    id: number;
    name: string;
    salaryRate: number;
    longevityKoeff: number;
    koeff: string;
    maxLongevityKoeff: number;
    isChildrenSalary: boolean;
}

interface IPositionTable {
    isEdit(is: boolean, id: number): void
    refresh: boolean
    setRefresh(refresh: boolean): void
}

const PositionsTable: React.FC<IPositionTable> = ({ isEdit, refresh, setRefresh }) => {
   const url = "api/Position";
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
                name: 'Наименование',
                minWidth: 70,
                maxWidth: 100,
                fieldName: 'name',
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                isPadded: true,
            },
            {
                key: 'salaryRate',
                name: 'Ставка',
                fieldName: 'salaryRate',
                minWidth: 70,
                maxWidth: 90,
                isResizable: true,
                isCollapsible: true,
                data: 'number',
            },
            {
                key: 'longevityKoeff',
                name: '% за каждый год работы',
                fieldName: 'longevityKoeff',
                minWidth: 100,
                maxWidth: 200,
                isResizable: true,
                isCollapsible: true,
                data: 'number',
            },
            {
                key: 'maxLongevityKoeff',
                name: 'Масимальный % за выслугу лет',
                fieldName: 'maxLongevityKoeff',
                minWidth: 100,
                maxWidth: 200,
                isResizable: true,
                isCollapsible: true,
                data: 'number',
       },
       {
           key: 'isChildrenSalary',
           name: 'Надбавка с зарплаты подчиненных',
           fieldName: 'isChildrenSalary',
           minWidth: 70,
           maxWidth: 90,
           isResizable: true,
           isCollapsible: true,
           data: 'boolean',
           onRender: (item: IPositionItem) => ((item.isChildrenSalary)?"Да":"Нет")
       },
    ];
   const emptyItems: IPositionItem[] = [];
   const _selection = new Selection({
       onSelectionChanged: () => {
           var id = _selection.getSelection()[0] === undefined ? 0 : (_selection.getSelection()[0] as IPositionItem).id
           if (id === 0) {
               isEdit(false, 0)
               return;
           }
           isEdit(true, id)
           }
        });
    const [items, setItems] = React.useState(emptyItems)
    const _setItems = (data: IPositionItem[], status: number) => {
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
   return (
       <div>

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

export default PositionsTable
