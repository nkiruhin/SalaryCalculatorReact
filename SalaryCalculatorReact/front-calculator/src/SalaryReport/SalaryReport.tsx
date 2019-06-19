import React from 'react';
import { Breadcrumb, IComboBoxOption, CommandBar, Dialog, DialogType, Stack, MessageBarType, Persona, PersonaSize} from 'office-ui-fabric-react';
import { DialogFooter } from 'office-ui-fabric-react/'
import { PrimaryButton, DefaultButton } from 'office-ui-fabric-react/'
import { ComboBox, TextField, IBreadcrumbItem } from 'office-ui-fabric-react/'
import { DetailsList, SelectionMode, IColumn } from 'office-ui-fabric-react/'
import fetchApi from '../fetcher';
import { Message, IMessage } from '../Main/Message';
const _months: IComboBoxOption[] = [
    { key: 1, text: 'Январь' },
    { key: 2, text: 'Февраль'},
    { key: 3, text: 'Март' },
    { key: 4, text: 'Апрель'},
    { key: 5, text: 'Май' },
    { key: 6, text: 'Июнь' },
    { key: 7, text: 'Июль' },
    { key: 8, text: 'Август' },
    { key: 9, text: 'Сентябрь' },
    { key: 10, text: 'Октябрь' },
    { key: 11, text: 'Ноябрь' },
    { key: 12, text: 'Декабрь' }
];

interface ISalaryReportItem {
    employeeName: string;
    positionName: string;
    dateCreate: string;
    sumOfSalary: string;
    salaryOfEmploees: string;
    period: string;
}
const columns: IColumn[] = [
    {
        key: 'employeeName',
        name: 'ФИО сотрудника',
        minWidth: 210,
        maxWidth: 350,
        isRowHeader: true,
        isResizable: true,
        data: 'string',
        isPadded: true,
        onRender: (item: ISalaryReportItem) => (<Persona text={item.employeeName}  size={PersonaSize.size32} />)
    },
    {
        key: 'positionName',
        name: 'Должность',
        minWidth: 100,
        maxWidth: 102,
        fieldName: 'positionName',
        isRowHeader: true,
        isResizable: true,
        data: 'string',
        isPadded: true,
    },
    {
        key: 'dateCreate',
        name: 'Дата расчета',
        fieldName: 'dateCreate',
        minWidth: 70,
        maxWidth: 90,
        isResizable: true,
        isCollapsible: true,
        data: 'number',
    },
    {
        key: 'sumOfSalary',
        name: 'Начисленная сумма',
        fieldName: 'sumOfSalary',
        minWidth: 70,
        maxWidth: 90,
        isResizable: true,
        isCollapsible: true,
        data: 'string',
    },
    {
        key: 'salaryOfEmploees',
        name: 'Сумма начисленний подчиненных',
        fieldName: 'salaryOfEmploees',
        minWidth: 70,
        maxWidth: 90,
        isResizable: true,
        isCollapsible: true,
        data: 'string',
    },
];
var getData = {
    month:'',
    year: ''
}



const breadcrumbItemsInit:IBreadcrumbItem[] = [
    { text: 'Отчет по зарплате', key: 'SalaryReport' },
    { text: 'Расчетный период', key: 'Period' }]
export const SalaryReport: React.FC = () => {
    const _emptyItems: ISalaryReportItem[] = [];
    const _startMessage: IMessage = { message: 'Для просмотра отчета задайте период', messageType: MessageBarType.info, show: true }
    const _commandBarItems = [
        {
            key: 'setPeriod', name: 'Период', ariaLabel: 'Задать период', onClick: () => _setPeriod(),
            iconProps: {
                iconName: 'Calendar'
            },
        },
        {
            key: 'loadReport', name: 'Показать отчет', ariaLabel: 'Показать', onClick: () => _loadReport(),
            iconProps: {
                iconName: 'CRMReport'
            }           
        },
         {
            key: 'Calculate', name: 'Расчет з/п', ariaLabel: 'Расчитать ', onClick: () => _calculate(),
            iconProps: {
                iconName: 'Calculator'
            }
        }
    ]
    const [hiddenDialog, setHiddenDialog] = React.useState(false)
    const [tableItems, setTableItems] = React.useState(_emptyItems)
    const [breadcrumbItems] = React.useState(breadcrumbItemsInit)
    const [message, setMessage] = React.useState<IMessage>(_startMessage);
    const _handleYear = (evn: any, value?: string) => {
        if (value) {
            getData.year = value
        };       
    }
    const _validateYear = (value: string): string => {
        if (value) { console.log(+value) };
        let year = new Date().getFullYear()
        if (+value > year) {
            return "Год не может быть больше текущего"
        } else if (+value < 1985) {
            return "Год не может быть меньше 1985"
        }
        return ""
    }
    const _handleMonth = (evn: any, options?: IComboBoxOption) => {
        if (options) { getData.month = (+options.key).toString() };
    }
    const _closeDialog = () => {
        if (breadcrumbItemsInit.length > 2) { breadcrumbItemsInit.pop() }
        breadcrumbItemsInit.push({ text: getData.month + '.' + getData.year, key: 'setPeriod' })
        if (getData.month !== '' && getData.year !== '') {
            _loadReport()
        }
        setHiddenDialog(true)
    }
    const _setPeriod = () => {
        console.log("Здесь задаем период")        
        setHiddenDialog(false)        
    }
    const _loadReport = () => {
        if (getData.month === '' && getData.year === '') {
            if (message.show) { setMessage({ show: false }) }
            setMessage({ message: "Не задан период", messageType: MessageBarType.error, show: true })
            return;
        }
        let url = "api/SalaryReport?month=" + getData.month + "&year=" + getData.year;
        fetchApi(_setItems, url, "GET")
    }
    const _calculate = () => {
        console.log("Здесь запускаем расчет")
        if (getData.month === '' && getData.year === '') {
            if (message.show) { setMessage({ show: false }) }
            setMessage({ message: "Не задан период", messageType: MessageBarType.error, show: true })
            return;
        }
        let url = "api/SalaryReport/Calculate?month=" + getData.month + "&year=" + getData.year;
        fetchApi(_loadReport, url, "POST")
    }
    const _setItems = (data: any , status: number) => {
        if (status === 200) {
            setTableItems(data as ISalaryReportItem[])
            if (message.show) { setMessage({ show: false }) }
        } else {
            console.log("Ошибка загрузки данных")
            setMessage({ message: data.error, messageType: MessageBarType.error, show: true })
            setTableItems(_emptyItems)
        }
    }
    if (localStorage.Role !== "Administrator") {
        _commandBarItems.pop()
    }
    return <div>
        <Dialog
            hidden={hiddenDialog}
            minWidth={ "470px" }
            onDismiss={_closeDialog}
            dialogContentProps={{
                type: DialogType.normal,
                title: "Период"
            }}> 
            <Stack horizontal={true} tokens={{ padding: '2px', childrenGap: '20px' }} wrap={true} horizontalAlign={"baseline"} >
                <div>
                <ComboBox
                        label={"Месяц"}
                        options={_months}
                        required={true}
                        useComboBoxAsMenuWidth={true}
                        onChange={_handleMonth}
                    />
                </div>
                <div>
                <TextField
                        label="Год"
                        required={true}
                        type={"number"}
                        onChange={_handleYear}
                        onGetErrorMessage={_validateYear}
                    />
                </div>
            </Stack>
            <DialogFooter>
                <PrimaryButton onClick={_closeDialog} text="Сохранить" />
                <DefaultButton onClick={_closeDialog} text="Отмена" />
            </DialogFooter>
        </Dialog>
        <Breadcrumb
            items={breadcrumbItems}
        />
        <CommandBar
            items={_commandBarItems}
            ariaLabel={'Use left and right arrow keys to navigate between commands'}
        />
        <Message {...message} />
        <div style={{
            height: '600px',
            position: 'relative',
            maxHeight: '40%',
            overflow: 'auto'
        }}>
        <DetailsList
            items={tableItems}
            columns={columns}
            selectionMode={SelectionMode.none}
           
        /></div>
    </div>
}