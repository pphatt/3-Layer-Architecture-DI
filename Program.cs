namespace ThreeLayerArchitectureDIDemo;

#region Models

public class Student 
{
    public string Name { get; set; }
    public int Age { get; set; }
}

#endregion Models

#region Interfaces

public interface IStudentRepository
{
    void AddStudent(string className, Student student);
    void RemoveStudent(string className, string studentName);
    Student? GetStudentByNameAndClass(string className, string studentName);
    List<Student>? GetAllStudentsByClass(string className);
}

public interface IStudentService
{
    void AddStudentToClass(string className, Student student);
    void RemoveStudentFromClass(string className, string studentName);
    Student? ViewStudentDetails(string className, string studentName);
    List<Student>? ViewStudentsInClass(string className);
}

#endregion Interfaces

#region Repository

public class StudentRepository : IStudentRepository
{
    private readonly Dictionary<string, List<Student>> _classes = new();

    public StudentRepository()
    {
    }

    public void AddStudent(string className, Student student)
    {
        if (!_classes.ContainsKey(className))
        {
            _classes[className] = new();
        }

        _classes[className].Add(student);
    }

    public List<Student>? GetAllStudentsByClass(string className)
    {

        if (_classes.ContainsKey(className))
        {
            return _classes[className];
        }
        else
        {
            Console.WriteLine($"{className} class is not found in the system.");
            return null;
        }
    }

    public Student? GetStudentByNameAndClass(string className, string studentName)
    {
        if (!_classes.ContainsKey(className))
        {
            Console.WriteLine($"{className} class is not found in the system.");
            return null;
        }

        return _classes[className].FirstOrDefault(s => s.Name.Equals(studentName, StringComparison.OrdinalIgnoreCase));
    }

    public void RemoveStudent(string className, string studentName)
    {
        if (!_classes.ContainsKey(className))
        {
            Console.WriteLine($"{className} class is not found in the system.");
            return;
        }

        var student = GetStudentByNameAndClass(className, studentName);

        if (student is null)
        {
            Console.WriteLine($"{studentName} not found in {className} class");
            return;
        }

        _classes[className].Remove(student);
        Console.WriteLine($"{studentName}: {studentName}");
    }
}

#endregion Repository

#region Services

public class StudentService : IStudentService
{
    private IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public void AddStudentToClass(string className, Student student)
    {
        _repository.AddStudent(className, student);
    }

    public void RemoveStudentFromClass(string className, string studentName)
    {
        _repository.RemoveStudent(className, studentName);
    }

    public Student? ViewStudentDetails(string className, string studentName)
    {
        return _repository.GetStudentByNameAndClass(className, studentName);
    }

    public List<Student>? ViewStudentsInClass(string className)
    {
        return _repository.GetAllStudentsByClass(className);
    }
}

#endregion Services

#region Controllers

public class StudentController
{
    private IStudentService _service;

    public StudentController(IStudentService service)
    {
        _service = service;
    }

    public void AddStudent(string className, Student student)
    {
        _service.AddStudentToClass(className, student);
    }

    public void RemoveStudent(string className, string studentName)
    {
        _service.RemoveStudentFromClass(className, studentName);
    }

    public List<Student>? GetStudentsByClass(string className)
    {
        return _service.ViewStudentsInClass(className);
    }

    public Student? GetStudentDetails(string className, string studentName)
    {
        return _service.ViewStudentDetails(className, studentName);
    }
}

#endregion

#region Presentaion

public class StudentPresentation
{
    private readonly StudentController _controller;

    public StudentPresentation(StudentController controller)
    {
        _controller = controller;
    }

    public void InitMenu()
    {
        Console.WriteLine("Welcome to the Student Management Console\n");
        Console.WriteLine("Options: Add, Remove, View, View Details, Exit");

        while (true)
        {
            Console.Write("\nEnter your choice: ");
            string? choice = Console.ReadLine()?.Trim().ToLower();

            switch (choice)
            {
                case "add":
                    AddStudent();
                    break;
                case "remove":
                    RemoveStudent();
                    break;
                case "view":
                    ViewStudents();
                    break;
                case "view details":
                    ViewStudentDetails();
                    break;
                case "exit":
                    Console.WriteLine("Exiting... Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void AddStudent()
    {
        Console.Write("Enter class name (e.g., A1): ");
        string? className = Console.ReadLine();

        if (string.IsNullOrEmpty(className))
        {
            Console.WriteLine("Invalid class name input.");
            return;
        }

        Console.Write("Enter student name: ");
        string? studentName = Console.ReadLine();

        if (string.IsNullOrEmpty(studentName))
        {
            Console.WriteLine("Invalid student name input.");
            return;
        }

        Console.Write("Enter student age: ");
        if (!int.TryParse(Console.ReadLine(), out int age))
        {
            Console.WriteLine("Invalid student age input.");
            return;
        }

        _controller.AddStudent(className, new Student { Name = studentName, Age = age });
        Console.WriteLine("Student added successfully!");
    }

    private void RemoveStudent()
    {
        Console.Write("Enter class name (e.g., A1): ");
        string? className = Console.ReadLine();

        if (string.IsNullOrEmpty(className))
        {
            Console.WriteLine("Invalid class name input.");
            return;
        }

        Console.Write("Enter student name to remove: ");
        string? studentName = Console.ReadLine();

        if (string.IsNullOrEmpty(studentName))
        {
            Console.WriteLine("Invalid student name input.");
            return;
        }

        _controller.RemoveStudent(className, studentName);
        Console.WriteLine("Student removed successfully!");
    }

    private void ViewStudents()
    {
        Console.Write("Enter class name (e.g., A1): ");
        string? className = Console.ReadLine();

        if (string.IsNullOrEmpty(className))
        {
            Console.WriteLine("Invalid class name input.");
            return;
        }

        var students = _controller.GetStudentsByClass(className);

        if (students is not null && students.Any())
        {
            Console.WriteLine($"Students in {className}:");
            foreach (var student in students)
            {
                Console.WriteLine($"- {student.Name}, Age: {student.Age}");
            }
        }
        else
        {
            Console.WriteLine("No students found in this class.");
        }
    }

    private void ViewStudentDetails()
    {
        Console.Write("Enter class name (e.g., A1): ");
        string? className = Console.ReadLine();

        if (string.IsNullOrEmpty(className))
        {
            Console.WriteLine("Invalid class name input.");
            return;
        }

        Console.Write("Enter student name: ");
        string? studentName = Console.ReadLine();

        if (string.IsNullOrEmpty(studentName))
        {
            Console.WriteLine("Invalid student name input.");
            return;
        }

        var student = _controller.GetStudentDetails(className, studentName);

        if (student is not null)
        {
            Console.WriteLine($"Student Details: {student.Name}, Age: {student.Age}");
        }
        else
        {
            Console.WriteLine("No student found with the given details.");
        }
    }
}

#endregion Presentaion

internal class Program
{
    static void Main(string[] args)
    {
        // Dependency Injection Setup
        IStudentRepository studentRepository = new StudentRepository();
        IStudentService studentService = new StudentService(studentRepository);
        StudentController studentController = new StudentController(studentService);
        StudentPresentation studentView = new StudentPresentation(studentController);

        studentView.InitMenu();
    }
}
