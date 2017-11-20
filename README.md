# Principles of Web Development (comp307) - Final Project

*This project was made for the class Comp307 at McGill University (Fall 2017).*

## Authors
**Isabelle Grégoire**  
isabelle.gregoire3@mail.mcgill.ca  
**Simon Fredette**  
simon.fredette@mail.mcgill.ca  
**Arjun B. Gupta**  
boris.gupta@mail.mcgill.ca

## Description
#### Ruby version
Running with:
ruby 2.4.1p111 (2017-03-22 revision 58053) [x86_64-darwin16]

#### System Configuration
* This app is working on macOS Sierra Version 10.12.6.
* Using PostgreSQL 9.6.
* Database name has to be `comp307`.

#### Database
Using postgres as our DBS.  
Commands used for db creation:
* `$ rails generate model Game name:string dev:string fund_goal:integer current_fund:integer nmb_backers:integer funding_period:integer`

#### Main instructions
* Boot the postgres database.
* Run `$ rails s` command in the terminal to start the server.
* Go to your browser and type "localhost:3000" as a URL.