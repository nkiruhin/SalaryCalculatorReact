import React from 'react';
import './App.css';
import { Fabric } from 'office-ui-fabric-react/lib/Fabric';
import { BrowserRouter, Switch, Route } from 'react-router-dom'
import { AppSidebar } from './Sidebar'
import { Employees } from './Employees/Employees';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import { Positions } from './Positions/Positions';
import { SalaryReport } from './SalaryReport/SalaryReport';
import { HeadMenu } from './HeadMenu';

initializeIcons(/* optional base url */);

const App: React.FC = () => (
    <Fabric>       
        <div className="App">
            <BrowserRouter>
                <div className="header">
                    <HeadMenu />
                </div>
                <div className="body">
                    <div className="sidebar"><AppSidebar/></div>
                    <div className="content">
                        <Switch>
                            <Route exact={true} path='/' component={ Employees } />
                            <Route path='/positions' component={Positions} />
                            <Route path='/salaryreports' component={SalaryReport} />
                        </Switch>
                    </div>
                </div>
            </BrowserRouter>
            <div className="footer" />
        </div>
    </Fabric>
  );


export default App;
