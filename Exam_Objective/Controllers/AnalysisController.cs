using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Mime;
using System.IO;
using System.Text;
using Exam_Objective.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using ConsoleAppLog;
using Networks;



namespace Exam_Objective.Controllers
{
    public class AnalysisController : Controller
    {
        // GET: Analysis
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestAnalysis()
        {
            return View();
        }

        public ActionResult Analysis()
        {
            return View();
        }

        public ActionResult test()
        {
            return View();
        }

        public ActionResult EvaluationIOC()
        {
            return View();
        }

        public ActionResult ShowAnalysisTest()
        {
            return View();
        }

        public ActionResult HeaderTestAnalysis()
        {
            return View();
        }

        public ActionResult jsonReadFile(AnalysisModel Ana )
        {
            List<AnalysisModel> Numlist = new List<AnalysisModel>();


            string st = Ana.name;
            List<string> La = new List<string>();
            List<double> La1 = new List<double>();

            int aL = 0, bL = 0, cL = 0, dL = 0;
            int aM = 0, bM = 0, cM = 0, dM = 0;
            int aH = 0, bH = 0, cH = 0, dH = 0;
            double p1 = 0, p2 = 0, p3 = 0, p4 = 0;

            if (Ana.name != null)
            {
                int a = Ana.name.Length;
                a = (a - 1) / 159;
                int sel = 159;
                string[] str = new string[a];
                for (int i = 0; i < a; i++)
                {
                    str[i] = Ana.name.ToString().Substring(sel * i, 159);
                    str[i] = str[i].ToString().Substring(57, 100);
                }

                int count = 0;

                //ตรวจคะแนน
                for (int i = 1; i < a; i++)
                {
                    for (int j = 0; j < a; j++)
                    {
                        if (str[0].Substring(j, 1) == str[i].Substring(j, 1))
                        {
                            count++;
                        }
                    }
                    str[i] = count.ToString() + " : " + str[i].Substring(0, 100);
                    count = 0;
                }
                Array.Sort(str);

                //แยกกลุ่มสูงกลุ่มต่ำ
                //int[] CountCh = new int[str.Length];
                for (int j = 0; j < str.Length; j++)
                {
                    for (int i = 1; i < str.Length; i++)
                    {   //กลุ่มต่ำ
                        if (i < str.Length - 73)
                        {
                            if (str[i].Substring(5 + j, 1) == "1")
                            {
                                aL++;
                            }
                            if (str[i].Substring(5 + j, 1) == "2")
                            {
                                bL++;
                            }
                            if (str[i].Substring(5 + j, 1) == "3")
                            {
                                cL++;
                            }
                            if (str[i].Substring(5 + j, 1) == "4")
                            {
                                dL++;
                            }
                        }
                        else if (i < str.Length - 27)
                        {   //กลุ่มกลาง
                            if (str[i].Substring(5 + j, 1) == "1")
                            {
                                aM++;
                            }
                            if (str[i].Substring(5 + j, 1) == "2")
                            {
                                bM++;
                            }
                            if (str[i].Substring(5 + j, 1) == "3")
                            {
                                cM++;
                            }
                            if (str[i].Substring(5 + j, 1) == "4")
                            {
                                dM++;
                            }
                        }
                        else
                        {   //กลุ่มสูง
                            if (str[i].Substring(5 + j, 1) == "1")
                            {
                                aH++;
                            }
                            if (str[i].Substring(5 + j, 1) == "2")
                            {
                                bH++;
                            }
                            if (str[i].Substring(5 + j, 1) == "3")
                            {
                                cH++;
                            }
                            if (str[i].Substring(5 + j, 1) == "4")
                            {
                                dH++;
                            }
                        }
                    }
                    p1 = (aH + aL + bM) / (str.Length);
                    p2 = (bH + bL + bM) / (str.Length);
                    p3 = (cH + cL + cM) / (str.Length);
                    p4 = (dH + dL + dM) / (str.Length);
                    //Numlist.Add(new AnalysisModel { aHigh = aH, aMid = aM, aLow = aL, bHigh = bH, bMid = bM, bLow = bL, cHigh = cH, cMid = cM, cLow = cL, dHigh = dH, dMid = dM, dLow = dL, pA = p2, pB = p2, pC = p3, pD = p4 });


            La1.Add(444);


                    aL = 0; bL = 0; cL = 0; dL = 0;
                    aM = 0; bM = 0; cM = 0; dM = 0;
                    aH = 0; bH = 0; cH = 0; dH = 0;
                    p1 = 0; p2 = 0; p3 = 0; p4 = 0;
                }
               // ViewBag.tesV = Numlist;
            }

            La.Add("sssss");

            ViewBag.data2 = La;
            ViewData["data1"] = La1;


            //TempData["Numlist"] = Numlist;
            //return PartialView();
            return View();
            //Ana, JsonRequestBehavior.AllowGet
        }
    }
}