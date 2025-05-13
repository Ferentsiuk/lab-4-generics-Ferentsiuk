# Лабораторна робота №4 – Generics

**Тема:** «Побудова generic-репозиторію та моделі “Факультет → Група → Студент/Викладач”»

---

## Мета роботи

* Навчитися поступово розширювати один generic-клас репозиторію:

  * базовий CRUD-інтерфейс,
  * накладати обмеження `where`,
  * реалізувати read-only варіант з коваріантністю (`out`),
  * write-only варіант з контраваріантністю (`in`).
* Одночасно змоделювати процес взаємодії студентів і викладачів у рамках груп і факультету.

---

## Початкові умови

* Ви — студент(ка) ФПМ КПІ (групи КП-41, КП-42, КП-43).
* Створити базовий клас **Person**:

  ```csharp
  class Person { public int Id; public string Name; }
  ```
* Наслідувачі **Student** і **Teacher**:

  * **Student**: методи `SubmitWork()`, `SayName()`.
  * **Teacher**: методи `GradeStudent(...)`, `ExpelStudent(...)`, `ShowPresentStudents()`.

---

## Структура роботи

### Етап 1. Початкова версія generic-репозиторію

1. **Класи сутностей**

   * \`class Student { int Id; string Name; /\* SubmitWork(), SayName() \*/ }
   * \`class Teacher { int Id; string Name; /\* GradeStudent(), ExpelStudent(), ShowPresentStudents() \*/ }

2. **Інтерфейс CRUD**

   ```csharp
   interface IRepository<TEntity, TKey> {
     void Add(TKey id, TEntity entity);
     TEntity Get(TKey id);
     IEnumerable<TEntity> GetAll();
     void Remove(TKey id);
   }
   ```

3. **InMemoryRepository**

   * Реалізувати `class InMemoryRepository<TEntity,TKey> : IRepository<TEntity,TKey>`.
   * Використати `Dictionary<TKey,TEntity>` для зберігання.

---

### Етап 2. Побудова базової моделі факультету і груп

1. **Group**

   * Властивості: `int Id`, `string Name`.
   * Поле: `IRepository<Student,int> _students = new InMemoryRepository<Student,int>();`
   * Методи:

     * `AddStudent(Student s)`
     * `RemoveStudent(int studentId)`
     * `IEnumerable<Student> GetAllStudents()`
     * `Student FindStudent(int studentId)`

2. **Faculty**

   * Властивості: `int Id`, `string Name`.
   * Поле: `IRepository<Group,int> _groups = new InMemoryRepository<Group,int>();`
   * Методи:

     * `AddGroup(Group g)`, `RemoveGroup(int id)`
     * `IEnumerable<Group> GetAllGroups()`, `Group GetGroup(int id)`
     * Через групу:

       * `AddStudentToGroup(int groupId, Student s)`
       * `RemoveStudentFromGroup(int groupId, int studentId)`
       * `IEnumerable<Student> GetAllStudentsInGroup(int groupId)`
       * `Student FindStudentInGroup(int groupId, int studentId)`

3. **Перевірка**

   * У `Main()` створити `Faculty fpm`, додати групи КП-41, КП-42, КП-43, створити декількох студентів,
     додати їх у групу та вивести список студентів.

---

### Етап 3. Додавання обмежень типів (`where`)

1. **Модифікація CRUD-інтерфейсу**

   ```csharp
   interface IRepository<TEntity, TKey>
     where TEntity : class, new()
     where TKey    : struct
   { /* … */ }
   ```
2. Переконатися, що `InMemoryRepository` компілюється під новими обмеженнями.
3. **Перевірка**

   * `IRepository<Student,int>` – компілюється.
   * `IRepository<int,int>` – викликає помилку компіляції.

---

### Етап 4\*. Read-only інтерфейс (коваріантність)

1. **Інтерфейс Read-only**

   ```csharp
   interface IReadOnlyRepository<out TEntity, in TKey> {
     TEntity Get(TKey id);
     IEnumerable<TEntity> GetAll();
   }
   ```
2. Додати реалізацію `IReadOnlyRepository<TEntity,TKey>` до `InMemoryRepository`.
3. **Перевірка коваріантності**

   ```csharp
   IReadOnlyRepository<Student,int> studRepo = new InMemoryRepository<Student,int>();
   IReadOnlyRepository<Person,int>  persRepo = studRepo;  // має компілюватися
   ```

---

### Етап 5\*. Write-only інтерфейс (контраваріантність)

1. **Інтерфейс Write-only**

   ```csharp
   interface IWriteRepository<in TEntity, in TKey> {
     void Add(TEntity entity);
     void Remove(TKey id);
   }
   ```
2. Реалізувати `IWriteRepository<TEntity,TKey>` у `InMemoryRepository`.
3. **Перевірка контраваріантності**

   ```csharp
   IWriteRepository<Person,int> persWrite = new InMemoryRepository<Student,int>();
   IWriteRepository<Student,int> studWrite = persWrite;  // має компілюватися
   ```

---

## Пояснення етапів

1–2: створення репозиторію та базової моделі факультету й груп;
3: додавання безпеки за допомогою `where`;
4–5: розділення операцій читання та запису та демонстрація механізмів `out`/`in`.

---

**Виконання:** реалізуйте всі класи й інтерфейси, проведіть перевірки у `Main()`, зафіксуйте результати компіляції та виконання.
