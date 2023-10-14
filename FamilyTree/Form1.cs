using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace FamilyTree
{
    public partial class Form1 : Form
    {
        private static bool entered;
        private static List<GroupBox>? gbList;
        public Form1()
        {
            InitializeComponent();
        }

        public static List<GroupBox>? GbList { get => gbList; set => gbList = value; }

        private void Form1_Load(object sender, EventArgs e)
        {
            Person.loadDatabase("backend.txt");
            Person.scaleDatabaseToSize();
            gbList = Person.addDatabaseToGroupBox(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (gbList == null) return;
            entered = false;
            foreach (GroupBox gb in gbList)
            {
                entered = gb.ClientRectangle.Contains(gb.PointToClient(Cursor.Position));
                if (entered)
                {
                    gb.Size = new Size(324, 275);
                    //break;
                }
                else
                {
                    gb.Size = new Size(145, 275);
                }
            }
        }

        public void addAndReloadDatabase(Person p, string path = "backend.txt")
        {
            this.Controls.Clear();
            Person.addPersonToFile(p, path);
            Person.loadDatabase(path);
            Person.scaleDatabaseToSize();
            gbList = Person.addDatabaseToGroupBox(this);
        }
    }

    public class Person : IEquatable<Person?>, IComparable<Person>
    {
        public static int people = 0;
        public static int groupWidth = 160;
        public static int groupHeight = 285;
        public static List<Person>? persons;

        private int id;
        private int level;
        private int savedLevel;
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

        public static GroupBox createGroupBox(Person p, Form1 f1)
        {
            GroupBox gb = new GroupBox();

            gb.Name = "Person_" + people;
            gb.Text = "Person";
            gb.Size = new Size(324, 275); //324 with options, 145 without
            gb.Location = new Point(12, 12);
            gb.BackColor = Color.FromArgb(255,
                (int)(255 * p.MotherGenes / 100.00),
                (int)(71 * p.FatherGenes / 100.00),
                (int)(171 * Math.Max(p.FatherGenes, p.MotherGenes) / 100.00));
            PictureBox pb = new PictureBox();
            pb.Size = new Size(100, 100);
            try
            {
                Image im = Image.FromFile(@"Pictures\" + p.Fullname + ".jpg");
                pb.Image = (Image)(new Bitmap(im, new Size(100, 100)));
            }
            catch
            {
                pb.BackColor = Color.Beige;
            }
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

            //adding Options controls
            GroupBox gb1 = new GroupBox();
            gb1.Size = new Size(157, 141); //+16 per
            gb1.Location = new Point(157, 12);
            Button b1 = new Button();
            b1.Text = "+ Parent";
            b1.Size = new Size(132, 23);
            b1.Location = new Point(13, 22);
            b1.BackColor = Color.LightGray;
            b1.Name = p.Fullname;
            b1.Click += (sender, e) => parent_click(sender, e, f1);
            gb1.Controls.Add(b1);
            Button b2 = new Button();
            b2.Text = "+ Sibling";
            b2.Size = new Size(132, 23);
            b2.Location = new Point(13, 50);
            b2.BackColor = Color.LightGray;
            b2.Name = p.Fullname;
            b2.Click += (sender, e) => sibling_click(sender, e, f1);
            gb1.Controls.Add(b2);
            Button b3 = new Button();
            b3.Text = "+ Child";
            b3.Size = new Size(132, 23);
            b3.Location = new Point(13, 76);
            b3.BackColor = Color.LightGray;
            b3.Name = p.Fullname;
            b3.Click += (sender, e) => child_click(sender, e, f1);
            gb1.Controls.Add(b3);
            Button b4 = new Button();
            b4.Text = "DELETE SELF";
            b4.Size = new Size(132, 23);
            b4.Location = new Point(13, 101);
            b4.BackColor = Color.LightGray;
            b4.ForeColor = Color.Red;
            b4.Name = p.Fullname;
            b4.Click += (sender, e) => delete_click(sender, e, f1);
            gb1.Controls.Add(b4);
            gb.Controls.Add(gb1);

            gb.Size = new Size(145, 275);
            gb.Location = new Point(p.LeftCoords, p.TopCoords);
            people++;
            return gb;
        }

        static void parent_click(object? sender, EventArgs e, Form1 f1)
        {
            //Get the button clicked
            Button? btn = sender as Button;
            //MessageBox.Show(btn.Name + " Clicked");

            if (persons == null || btn == null) return;
            Person pNew = new Person();
            bool b = false;
            foreach (Person p in persons)
            {
                if (p.Fullname != null && p.Fullname.Equals(btn.Name))
                {
                    b = true;
                    pNew.Level = p.Level - 1;
                    pNew.SavedLevel = p.SavedLevel - 1;
                    pNew.MotherGenes = p.MotherGenes / 2.0;
                    pNew.FatherGenes = p.FatherGenes / 2.0;
                    pNew.genesToMe = p.GenesToMe * 0.5;
                    pNew.BirthDate = p.BirthDate.AddYears(-20);
                    pNew.DeathDate = p.DeathDate.AddDays(1);
                    pNew.Fullname = "";
                    pNew.Title = "";
                    pNew.ManualShift = 0;
                }
            }
            if(b) persons.Add(pNew);
            f1.addAndReloadDatabase(pNew);
        }
        static void sibling_click(object? sender, EventArgs e, Form1 f1)
        {
            //Get the button clicked
            Button? btn = sender as Button;
            //MessageBox.Show(btn.Name + " Clicked");

            if (persons == null || btn == null) return;
            Person pNew = new Person();
            bool b = false;
            foreach (Person p in persons)
            {
                if (p.Fullname != null && p.Fullname.Equals(btn.Name))
                {
                    b = true;
                    pNew.Level = p.Level;
                    pNew.SavedLevel = p.SavedLevel;
                    pNew.MotherGenes = p.MotherGenes / 2.0;
                    pNew.FatherGenes = p.FatherGenes / 2.0;
                    pNew.genesToMe = p.GenesToMe * (p.GenesToMe == 100 ? 0.5 : 1);
                    pNew.BirthDate = p.BirthDate;
                    pNew.DeathDate = p.DeathDate.AddDays(1);
                }
            }
            if (b) persons.Add(pNew);
            f1.addAndReloadDatabase(pNew);
        }
        static void child_click(object? sender, EventArgs e, Form1 f1)
        {
            //Get the button clicked
            Button? btn = sender as Button;
            //MessageBox.Show(btn.Name + " Clicked");

            if (persons == null || btn == null) return;
            Person pNew = new Person();
            bool b = false;
            foreach (Person p in persons)
            {
                if (p.Fullname != null && p.Fullname.Equals(btn.Name))
                {
                    b = true;
                    pNew.Level = p.Level + 1;
                    pNew.SavedLevel = p.SavedLevel + 1;
                    pNew.MotherGenes = p.MotherGenes / 2.0;
                    pNew.FatherGenes = p.FatherGenes / 2.0;
                    pNew.genesToMe = p.GenesToMe * 0.5;
                    pNew.BirthDate = p.BirthDate.AddYears(20);
                    pNew.DeathDate = p.DeathDate.AddYears(20);
                }
            }
            if (b) persons.Add(pNew);
            f1.addAndReloadDatabase(pNew);
        }
        static void delete_click(object? sender, EventArgs e, Form1 f1)
        {
            //Get the button clicked
            Button? btn = sender as Button;
            //MessageBox.Show(btn.Name + " Clicked");

            if (persons == null || btn == null) return;
        }

        public static bool addPersonToFile(Person p, string path)
        {
            if(p==null) return false;
            TextWriter tw = new StreamWriter(path, append: true);
            tw.WriteLine();
            tw.WriteLine(p.SavedLevel);
            tw.WriteLine(p.MotherGenes);
            tw.WriteLine(p.FatherGenes);
            tw.WriteLine(p.GenesToMe);
            tw.WriteLine(p.Title);
            tw.WriteLine(p.Fullname);
            tw.WriteLine(String.Format("{0,2:D}",p.BirthDate.Day) + "." + String.Format("{0,2:D}", p.BirthDate.Month) + "." + p.BirthDate.Year);
            if (p.DeathDate > DateTime.Now)
            {
                tw.WriteLine("NULL");
            }
            else
            {
                tw.WriteLine(p.DeathDate.Day + "." + p.DeathDate.Month + "." + p.DeathDate.Year);
            }
            tw.WriteLine(p.ManualShift);

            //tw.WriteLine();
            tw.Close();
            return true;
        }

        public static List<GroupBox> addDatabaseToGroupBox(Form1 f1)
        {
            List<GroupBox> gbList = new List<GroupBox>();
            if (persons == null) return gbList;
            foreach (Person p in persons)
            {
                GroupBox gb = createGroupBox(p,f1);
                f1.Controls.Add(gb);
                gbList.Add(gb);
            }
            return gbList;
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
                        p.SavedLevel = p.Level;
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
                    /*case 9: //id
                        p.Id = Convert.ToInt32(((string)s).Trim());
                        break;*/
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
        public int Id { get => id; set => id = value; }
        public int SavedLevel { get => savedLevel; set => savedLevel = value; }

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