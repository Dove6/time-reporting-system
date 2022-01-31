# Time Reporting System

Web-based timesheet for employees and project managers. Written using ASP.NET Core.

A simple CRUD application for ["RAD Tools" course](https://usosweb.usos.pw.edu.pl/kontroler.php?_action=katalog2%2Fprzedmioty%2FpokazPrzedmiot&prz_kod=103D-INIIT-ISP-NTR&lang=en).

## Functional requirements

### Main screen
- allows for selecting a day,
- displays activity entries for the specified day,
- sums up total work time for the specified day,
- provides "View", "Add", "Edit" and "Remove" buttons for displayed entries,
- disallows modification of entries from months declared as "frozen" (awaiting inspection by the manager)

### Activity entry creation view
- requires choosing parent project for the activity,
- allows for choosing a category withing that project,
- disallows choosing an inactive (closed) project,
- requires specifying how much time (in minutes) the activity took,
- allows for supplying additional information in the form of textual description

 ### Monthly summary view
- displays time summery for each project,
- allows for "freezing" the month,
- displays amount of time "accepted" (verified and corrected) by managers of each project

 ### Manager views
- project creation:
    - any user can create a project and become its manager,
- project summary:
    - managers can view total time each employee spent working on the project in each month,
    - if a month was marked as "frozen" by an employee, manager can "accept" work time reported by that employee (optionally, modify the value),
    - managers can view remaining time budget of their projects (which may be negative)
