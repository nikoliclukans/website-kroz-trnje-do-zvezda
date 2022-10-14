# Front-end HBS Starter v3.2.3

## Version history
<details open>
<summary>3.2.2</summary>

- packages are updated (07.09.2022.)
- added scroll-behavior-polyfill
</details>

<details>
<summary>3.2.1</summary>

- Hover mixin updated
</details>

<details>
<summary>3.2.0</summary>

- Compile RTE styles to the separate file
- Update HBS template placeholder layout
- Bug Fix: Fix multilang file path
</details>

<details>
<summary>3.1.0</summary>

- Fluid grid added
</details>

<details>
<summary>3.0.0</summary>

- All packages are updated (15.06.2022.)
- **Handlebars**
  - new hbs type - Partials
</details>

---

### Installation Instructions

Make sure you are on the correct version of Node.js. Required version is noted in the `package.json` under `engines.node`.
Install dependencies by running Node Package Manager. `npm install`

### Tasks

#### `npm run build-dev`
  - Builds the project for development purposes
  - It will add noindex, nofollow and will **not** optimize css and js files.
#### `npm run build-stage`
  - Builds the project for staging purposes
  - It will add noindex, nofollow and will opitimize css and js files.
#### `npm run build-prod`
  - Builds the project for production purposes
  - It will add index, follow, GTM and will optimize css and js files.
#### `npm run reset-dev`
  - Deletes `wwwroot` or `dist` folder. In order to run this command successfully make sure folder is not in use.
#### `npm run watch`
  - Turns on watch mode and compiles `.scss`, `.js` and `.hbs` files
#### `npm run cf`
  - Creates new files. It should be use to generate new handlebars templates, modules and partials
  - More info about this command can be found [here](#create-files)
#### `npm run iconfont`
  - Builds iconfont from `src/assets/svg` folder
  - This command will generate `.woff` and `.woff2` font files
  - Naming convention for the svg files should be `ico-[name].svg`
  - In order to show icons, all you need to do is add class `"icon font-ico-[name]"`
    - ```html
      <span class="icon font-ico-heart"></span>
      ```
#### `npm run css`
  - Builds css files from `src/scss` folder
#### `npm run js`
  - Runs webpack to compile javascript files from `src/js` folder
  - This command **will** run eslint too, so it will fail if there are errors
#### `npm run hbs`
  - Builds html files from templates in `src/html/templates` folder
#### `npm run assets`
  - Copies content of `src/assets` folder to `wwwroot/assets`



### Create files
#### Create module
Modules are used for static modules that are going to be used and reused throughtout the website.

Command: `npm run cf -- -m module-name`

This command will create new module files in `src/html/modules` directory:
* .hbs
* .json

And also a file in `src/scss/modules` directory:
* .scss

It will also update `style.scss` file in `src/scss` directory.

If you use `isMultilanguage` global variable, you will need to run `npm run add-lng` to generate missing languange files


#### Create partial
Partials are used for content that should be lazy loaded, like search results, load more results etc

Command: `npm run cf -- -p partial-name`

This command will create new partial files in `src/html/partials` directory:
* .hbs
* .json

And also a file in `src/scss/modules` directory:
* .scss

It will also update `style.scss` file in `src/scss` directory.

If you use `isMultilanguage` global variable, you will need to run `npm run add-lng` to generate missing languange files


#### Create template
Templates are used for pages, they will be converted in final html files

Command `npm run cf -- -t template-name`

This command will create new template files in `src/html/templates` directory:
* .hbs
* .json

If you use `isMultilanguage` global variable, you will need to run `npm run add-lng` to generate missing languange files

### Usage

#### HBS
##### Modules/Partials
All sections, modules, blocks, ... *(header, footer, banner...)* should be created as modules in `src/html/modules` and `src/scss/modules` directory.
All sections, modules, blocks that are going to be fetched from the server should be created as partials in `src/html/partials` and `src/scss/modules` directory.

Each module/partial has three files:

* `src/html/(modules/partial)` directory:
    * .hbs
    * .json

* `src/scss/modules` directory:
    * .scss

**Example:**

*.hbs file:*
```handlebars
<div class="team-list">
  <h2>{{title}}</h2>

  {{#if people}}
    <ul>
      {{#each people}}
        <li>{{this.name}} {{this.surname}}</li>
      {{/each}}
    </ul>
  {{/if}}
</div>
```

*.json file:*
```json
{
  "title": "Team List",
  "people": [
    {
      "name": "Emiko",
      "surname": "Groce"
    },
    {
      "name": "Leonila",
      "surname": "Gillins"
    }
  ]
}
```

*This will compile into:*
```handlebars
<div class="team-list">
  <h2>Team List</h2>

  <ul>
    <li>Emiko Groce</li>
    <li>Leonila Gillins</li>
  </ul>
</div>
```

See [Handlebars](https://handlebarsjs.com/) templating engine for more information about `.hbs` files.

##### Templates

Ideally all Templates should be created using *Modules*.

Each template has two files:
* .hbs *(template html templating structure)*
* .json *(template content)*

**Example:**

*.hbs file:*
```handlebars
{{#> master this}}
	{{> header data.header}}

	{{#each data.sections}}
		{{> (lookup this 'moduleAlias') this}}
	{{/each}}

	{{> footer data.footer}}
{{/master}}
```

*.json file:*
```json
{
	"template": "about",
	"bodyCssClass": "about",
	"data": {
		">>": "shared/master/data.json",
		"header": {
			">>": "shared/header/data.json"
		},
		"footer": {
			">>": "shared/footer/data.json"
		},
		"sections": [
			{
				"moduleAlias": "basic-block",
				">>": "modules/basic-block/data.json"
			},
			{
				"moduleAlias": "basic-block",
				">>": "modules/basic-block/data.json",
				"title": "Changed title"
			}
		]
	}
}
```

- Use `bodyCssClass` prop to set the body class for certain template *(`"clasbodyCssClasss": "about"` will add a `about` class to that template)*

- Use `data` prop object to:
- **include** module's `data.json` file:
```json
"header": {
    ">>": "shared/aheader/data.json"
}
```
- **include** module's .html file:
```json
"basicBlock2": {
    "content": "modules/basic-block/text.content.html"
}
```
- or **input** different data:
```json
"basicBlock2": {
    "title": "This diferent Title",
}
```
- or **include** modules's `data.json` file and override **only** data you need:
```json
"basicBlock3": {
    ">>": "basic-block/data.json",
    "title": "This is changed Title"
}
```

*This will compile into an `.html` file in `dist` directory.

#### CONDITIONS

The `compare` helper can be used where truthy or falsy data values are not enough, but you instead like to compare two data values, or compare something against a static value.

It supports all common operators, like `===`, `!==`, `<`, `<=`, `>`, `>=`, `&&` and `||`.

**Example:**

```handlebars
{{#if (compare v1 'operator' v2) }}
  foo
{{else if (compare v1 'operator' v2) }}
  bar
{{else}}
  baz
{{/if}}
```

Inline (nested) Condition Usage:

```handlebars
{{#if (compare (compare v1 'operator' v2) 'operator' (compare v1 'operator' v2))}}
  foo
{{else}}
  bar
{{/if}}
```


#### SCSS
All styles should be written in `src/scss` directories.

CSS code quality is checked with [Sass Lint](https://github.com/sasstools/sass-lint)

#### JavaScript
All scripts should be written in `src/js` directories.

Javascript code quality is checked with [ESLint](https://eslint.org/)

---

#### Config file
In this file are some variables that are used for or control the behavior of tasks.
Path: `src\config\config.js`

#### isMultilanguage
If you select this option hbs will get generated into separate folder for all of the languages.
**Important!** use this only if its a static website or an annual report.
To add more languages just updated the `globalVars.languages` array with new language.

#### Bugs and potential improvements
Please submit any bugs or potential improvements you find to [this](https://docs.google.com/forms/d/1flvR05hAps1fEL0ZYUs8Kd-ghU83JCFPnMlKcu5qBtU) Google form