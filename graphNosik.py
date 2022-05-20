import matplotlib.pyplot as plt
import tkinter as tk


def find_y(func:str, x:list) -> list:   
    """
        Возвращает значения фунции
    """
    if func.find("^") != -1: 
        func = func.replace("^", '**')
    y = []
    try:
        for i in x:
            l = func.replace('x', f'({str(i)})')
            y.append(eval(l))
        return(y)
    except:
        lb_ex.set('Введена некорректная функция.\nПоддерживаемые операции: +, -, *, /, ^')
        return(y)


def gr_build(f1:tk.StringVar, f2:tk.StringVar, xmin:tk.StringVar, xmax:tk.StringVar, step1:tk.StringVar):
    """
        Прорисовка графика
    """
    try:
        xmin0 = float(xmin.get())
    except:
        return(lb_ex.set('Xmin должно быть числом'))
    try:
        xmax0 = float(xmax.get())
        if xmax0 <= xmin0:
            return(lb_ex.set('Xmax должно быть числом > Xmin'))
    except:
        return(lb_ex.set('Xmax должно быть числом > Xmin'))
    try:       
        step0 = float(step1.get())
        if step0 <= 0:
            return(lb_ex.set('Значение шага должно быть числом > 0'))
    except:
        return(lb_ex.set('Значение шага должно быть числом > 0'))
   
    fun1 = f1.get()
    fun2 = f2.get()
    x1 = [] #здесь будут храниться значения Х, с заданным шагом
    k = xmin0
    while k <= xmax0:
        x1.append(k)
        k += step0
    y1 = find_y(fun1,x1)
    y2 = find_y(fun2,x1)
    if y1 != [] and y2 != []:
        plt.title(f"Зависимости: f1(x) = {fun1}, f2(x) = {fun2}") # заголовок
        plt.xlabel("x")         # ось абсцисс
        plt.ylabel("f1(x), f2(x)")    # ось ординат
        plt.grid()              # включение отображение сетки
        plt.plot(x1, y1, x1, y2)  # построение графика
        plt.show()
    else:
        return(lb_ex.set('Введена некорректная функция.\nПоддерживаемые операции: +, -, *, /, ^'))


mainwindow = tk.Tk()
mainwindow.title('Построитель графиков')
mainwindow.minsize(width= 560, height= 450)
mainwindow.maxsize(width= 560, height= 450)
func1 = tk.StringVar()
funk2 = tk.StringVar()
help1 = tk.StringVar()
lb_f1 = tk.StringVar()
lb_f2 = tk.StringVar()
lb_min = tk.StringVar()
lb_max = tk.StringVar()
en_xmin = tk.StringVar()
en_xmax = tk.StringVar()
lb_step = tk.StringVar()
en_step = tk.StringVar()
lb_step.set('Шаг = ')
en_step.set('0.01')
en_xmax.set('10')
en_xmin.set('-10')
lb_f1.set('f(x) = ')
lb_f2.set('f2(x) = ')
lb_min.set('Xmin = ')
lb_max.set('Xmax = ')
help1.set('Поддерживаемые операции: +, -, *, /, ^')

lbl = tk.Label(mainwindow, textvariable = help1, font = ("Times New Roman", 20))
lbl.place(x = 20, y = 20) #Поддерживаемые операции

lbl_f1 = tk.Label(mainwindow, textvariable = lb_f1, font = ("Times New Roman", 20))
lbl_f1.place(x = 20, y = 60)
entr_f1 = tk.Entry(mainwindow, textvariable = func1, width = 30, font = ("Times New Roman", 20))
entr_f1.place(x = 125, y = 60) #f(x)

lbl_f2 = tk.Label(mainwindow, textvariable = lb_f2, font = ("Times New Roman", 20))
lbl_f2.place(x = 20, y = 100)
entr_f2 = tk.Entry(mainwindow, textvariable = funk2, width = 30, font = ("Times New Roman", 20))
entr_f2.place(x = 125, y = 100) #f2(x)

lbl_xmin = tk.Label(mainwindow, textvariable = lb_min, font = ("Times New Roman", 20))
lbl_xmin.place(x = 20, y = 140)
lbl_xmax = tk.Label(mainwindow, textvariable = lb_max, font = ("Times New Roman", 20))
lbl_xmax.place(x = 20, y = 180)
entr_xmin = tk.Entry(mainwindow, textvariable = en_xmin, width = 30, font = ("Times New Roman", 20))
entr_xmin.place(x = 125, y = 140)
entr_xmax = tk.Entry(mainwindow, textvariable = en_xmax, width = 30, font = ("Times New Roman", 20))
entr_xmax.place(x = 125, y = 180) #xmin/xmax

lbl_step = tk.Label(mainwindow, textvariable = lb_step, font = ("Times New Roman", 20))
lbl_step.place(x = 20, y = 220)
entr_step = tk.Entry(mainwindow, textvariable = en_step, width = 30, font = ("Times New Roman", 20))
entr_step.place(x = 125, y = 220) #шаг

bt_build = tk.Button(mainwindow, text = "Построить", command=lambda: gr_build(func1, funk2, en_xmin, en_xmax, en_step), font = ("Times New Roman", 20))
bt_build.place(x = 200, y = 260)

lb_ex = tk.StringVar()
lbl_ex = tk.Label(mainwindow, textvariable = lb_ex, font = ("Times New Roman", 20))
lbl_ex.place(x = 20, y = 330)

mainwindow.mainloop()
