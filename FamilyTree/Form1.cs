using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace FamilyTree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Person.loadDatabase("backend.txt");
            Person.scaleDatabaseToSize();
            Person.addDatabaseToGroupBox(this);
        }
    }

    public class Person : IEquatable<Person?>, IComparable<Person>
    {
        public static int people = 0;
        public static int groupWidth = 160;
        public static int groupHeight = 285;
        public static List<Person>? persons;

        private int level;
        private double motherGenes;
        private double fatherGenes;
        private double genesToMe;
        private string? fullname;
        private string? title;
        private DateTime birthDate;
        private DateTime deathDate;
        private double manualShift;

        private int leftCoords;
        private int topCoords;

        public Person()
        {
        }

        public static DateTime loadDateTime(string s)
        {
            int day = Convert.ToInt32((s.Substring(0, 2)).Trim());
            int month = Convert.ToInt32((s.Substring(3, 2)).Trim());
            int year = Convert.ToInt32((s.Substring(6, 4)).Trim());
            return new DateTime(year, month, day);
        }

        public static GroupBox createGroupBox(Person p)
        {
            GroupBox gb = new GroupBox();

            gb.Name = "Person_" + people;
            gb.Text = "Person";
            gb.Size = new Size(145, 275); //324 with options
            gb.Location = new Point(12, 12);
            gb.BackColor = Color.FromArgb(255,
                (int)(255 * p.MotherGenes / 100.00),
                (int)(71 * p.FatherGenes / 100.00),
                (int)(171 * Math.Max(p.FatherGenes, p.MotherGenes) / 100.00));
            PictureBox pb = new PictureBox();
            pb.Size = new Size(100, 100);
            pb.BackColor = Color.Beige;
            pb.Location = new Point(23, 20);
            gb.Controls.Add(pb);
            TextBox tb1 = new TextBox();
            tb1.Text = p.Fullname;
            tb1.Location = new Point(6, 157);
            tb1.Multiline = true;
            tb1.Size = new Size(135, 40);
            tb1.TextAlign = HorizontalAlignment.Center;
            gb.Controls.Add(tb1);
            TextBox tb2 = new TextBox();
            tb2.Text = p.Title + " (" + p.genesToMe + "%)";
            tb2.Location = new Point(6, 128);
            tb2.Size = new Size(135, 23);
            tb2.TextAlign = HorizontalAlignment.Center;
            gb.Controls.Add(tb2);
            DateTimePicker dt1 = new DateTimePicker();
            dt1.Value = p.BirthDate;
            dt1.Location = new Point(6, 203);
            dt1.Size = new Size(135, 23);
            dt1.Format = DateTimePickerFormat.Short;
            gb.Controls.Add(dt1);
            DateTimePicker dt2 = new DateTimePicker();
            dt2.Value = p.DeathDate;
            dt2.Format = DateTimePickerFormat.Short;
            if (p.DeathDate > DateTime.Now)
            {
                dt2.Size = new Size(15, 23);
                dt2.Location = new Point(126, 232);
            }
            else
            {
                dt2.Location = new Point(6, 232);
                dt2.Size = new Size(135, 23);
            }
            gb.Controls.Add(dt2);


            gb.Location = new Point(p.LeftCoords, p.TopCoords);
            people++;
            return gb;
        }

        public static void addDatabaseToGroupBox(Form1 f1)
        {
            if (persons == null) return;
            foreach (Person p in persons)
            {
                f1.Controls.Add(createGroupBox(p));
            }
        }

        public static List<Person> scaleDatabaseToSize()
        {
            if (persons == null) return new List<Person>();
            if (persons.Count == 0) return persons;
            int minHeight = 2000000;
            int maxHeight = -2000000;
            //find smallest level, then scale so that level is == 0 level
            foreach (Person p in persons)
            {
                if (p.Level < minHeight)
                {
                    minHeight = p.Level;
                }
                if (p.Level > maxHeight)
                {
                    maxHeight = p.Level;
                }
            }
            maxHeight -= minHeight;

            //sort the levels in order: father genes 0% -> father genes 50% -> mother genes 50% -> mother genes 0%
            persons.Sort();

            double[] widths = new double[maxHeight + 1];
            //adjust the coordinates for width and height
            foreach (Person p in persons)
            {
                //height
                p.Level -= minHeight;
                p.topCoords = groupHeight * p.Level;

                //width
                p.LeftCoords = (int)widths[p.Level] + (int)(p.ManualShift * groupWidth);
                widths[p.Level] = p.LeftCoords + groupWidth;
            }

            //if (persons.Count > 0) throw new Exception();
            return persons;
        }

        public static List<Person> loadDatabase(string path)
        {
            persons = new List<Person>();

            TextReader tr = new StreamReader(path);
            //try
            //{
            string? s;
            int i = 0;
            Person p = new Person();
            while ((s = tr.ReadLine()) != null)
            {
                Console.WriteLine(s);
                if (s.Length >= 2 && s.Substring(0, 2) == "//") continue;
                switch (i)
                {
                    case 0: //level
                        p = new Person();
                        p.Level = Convert.ToInt32(((string)s).Trim());
                        break;
                    case 1: //mother genes
                        p.MotherGenes = Convert.ToDouble(((string)s).Trim());
                        break;
                    case 2: //father genes
                        p.FatherGenes = Convert.ToDouble(((string)s).Trim());
                        break;
                    case 3: //genes (compared to me)
                        p.GenesToMe = Convert.ToDouble(((string)s).Trim());
                        break;
                    case 4: //title
                        p.Title = s;
                        break;
                    case 5: //name
                        p.Fullname = s;
                        break;
                    case 6: //birth date
                        p.BirthDate = Person.loadDateTime(s);
                        break;
                    case 7: //death date
                        if (s != "NULL") p.DeathDate = Person.loadDateTime(s);
                        else p.DeathDate = DateTime.Now.AddDays(1);
                        break;
                    case 8: //manual shift
                        p.ManualShift = Convert.ToDouble(((string)s).Trim());
                        break;
                    case 9:
                        persons.Add(p);
                        i = -1;
                        break;
                    default:
                        throw new Exception("Entered illegal state.");

                }
                i++;
            }
            persons.Add(p);
            //}
            //catch (Exception ex) { Console.WriteLine(ex); }

            tr.Close();
            return persons;
        }
        public int CompareTo(Person? other)
        {
            if (other == null || other == this) return 0;
            else
            {
                if (fatherGenes == 0)
                {
                    return (this.MotherGenes > other.MotherGenes) ? -1 : 1;
                }
                return this.FatherGenes < other.FatherGenes ? 1 : -1;
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Person);
        }

        public bool Equals(Person? other)
        {
            return other is not null &&
                   level == other.level &&
                   motherGenes == other.motherGenes &&
                   fatherGenes == other.fatherGenes &&
                   genesToMe == other.genesToMe &&
                   fullname == other.fullname &&
                   title == other.title &&
                   birthDate == other.birthDate &&
                   deathDate == other.deathDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(level, motherGenes, fatherGenes, genesToMe, fullname, title, birthDate, deathDate);
        }

        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public DateTime DeathDate { get => deathDate; set => deathDate = value; }
        public int Level { get => level; set => level = value; }
        public double MotherGenes { get => motherGenes; set => motherGenes = value; }
        public double FatherGenes { get => fatherGenes; set => fatherGenes = value; }
        public double GenesToMe { get => genesToMe; set => genesToMe = value; }
        public string? Fullname { get => fullname; set => fullname = value; }
        public string? Title { get => title; set => title = value; }
        public int LeftCoords { get => leftCoords; set => leftCoords = value; }
        public int TopCoords { get => topCoords; set => topCoords = value; }
        public double ManualShift { get => manualShift; set => manualShift = value; }

        public static bool operator ==(Person? left, Person? right)
        {
            return EqualityComparer<Person>.Default.Equals(left, right);
        }

        public static bool operator !=(Person? left, Person? right)
        {
            return !(left == right);
        }
    }
}