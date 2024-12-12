import tkinter as tk  # Импорт библиотеки для создания графического интерфейса

# Функция для построения детерминированного конечного автомата (DFA) по шаблону
def build_automaton(pattern):
    pattern = pattern.lower()  # Приведение шаблона к нижнему регистру для унификации
    m = len(pattern)  # Длина шаблона
    if m == 0:  # Если шаблон пустой, вернуть начальное состояние и пустой алфавит
        return [{}], set()

    alphabet = set(pattern)  # Уникальные символы шаблона (алфавит)
    automaton = [{} for _ in range(m + 1)]  # Список состояний DFA
    prefix = [0] * m  # Префикс-функция для шаблона (используется для переходов)

    # Построение префикс-функции
    j = 0
    for i in range(1, m):
        while j > 0 and pattern[i] != pattern[j]:  # Возврат на предыдущие совпадения
            j = prefix[j - 1]
        if pattern[i] == pattern[j]:  # Расширение совпадения
            j += 1
        prefix[i] = j

    # Построение DFA
    for state in range(m + 1):
        for char in alphabet:
            if state < m and char == pattern[state]:  # Если символ соответствует шаблону
                automaton[state][char] = state + 1
            elif state > 0:  # Иначе использовать префикс-функцию
                automaton[state][char] = automaton[prefix[state - 1]].get(char, 0)
            else:  # Начальное состояние
                automaton[state][char] = 0

    return automaton, alphabet


# Функция для поиска подстроки в тексте с использованием DFA
def search_with_automaton(text, pattern):
    text = text.lower()  # Приведение текста к нижнему регистру
    pattern = pattern.lower()  # Приведение шаблона к нижнему регистру
    if not pattern:  # Если шаблон пустой, вернуть пустой список
        return []

    automaton, _ = build_automaton(pattern)  # Построить DFA
    state = 0  # Начальное состояние DFA
    matches = []  # Список найденных совпадений

    for i, char in enumerate(text):  # Проход по символам текста
        state = automaton[state].get(char, 0)  # Переход между состояниями
        if state == len(pattern):  # Если достигнуто конечное состояние (совпадение найдено)
            matches.append(i - len(pattern) + 1)

    return matches


# Функция для преобразования индекса в формат Tkinter
def index_to_tk_index(index, text):
    lines = text.split("\n")  # Разделение текста на строки
    current_index = index
    for line_number, line in enumerate(lines, start=1):  # Определение строки и позиции
        if current_index < len(line):
            return f"{line_number}.{current_index}"
        current_index -= len(line) + 1
    return f"{len(lines)}.{len(lines[-1])}"


# Основная функция для отображения DFA и выполнения поиска
def display_automaton_and_search():
    text = entry_text.get("1.0", tk.END).strip()  # Получение текста из поля
    pattern = entry_pattern.get().strip()  # Получение шаблона из поля

    if not text or not pattern:  # Проверка на наличие текста и шаблона
        results_label.config(text="Please enter valid text and pattern.")
        return

    automaton, alphabet = build_automaton(pattern)  # Построение DFA
    alphabet = sorted(alphabet)  # Сортировка алфавита для отображения

    # Очистка таблицы DFA
    for widget in frame_table.winfo_children():
        widget.destroy()

    # Создание заголовка таблицы
    header = ["State"] + alphabet
    for col, header_text in enumerate(header):
        tk.Label(frame_table, text=header_text, relief="ridge", width=10).grid(row=0, column=col)

    # Заполнение таблицы состояниями и переходами
    for state, transitions in enumerate(automaton):
        tk.Label(frame_table, text=state, relief="ridge", width=10).grid(row=state + 1, column=0)
        for col, char in enumerate(alphabet, start=1):
            value = transitions.get(char, 0)
            tk.Label(frame_table, text=value, relief="ridge", width=10).grid(row=state + 1, column=col)

    # Выполнение поиска и отображение результатов
    matches = search_with_automaton(text, pattern)
    results_label.config(text=f"Matches found: {len(matches)}. Positions: {matches}")

    # Подсветка совпадений в тексте
    entry_text.tag_remove("highlight", "1.0", tk.END)
    if matches:
        for match_start in matches:
            match_end = match_start + len(pattern)
            start_index = index_to_tk_index(match_start, text)
            end_index = index_to_tk_index(match_end, text)
            entry_text.tag_add("highlight", start_index, end_index)
        entry_text.tag_config("highlight", background="yellow", foreground="black")


# Интерфейс на Tkinter
root = tk.Tk()
root.title("DFA для поиска подстроки")  # Название окна
root.geometry("500x700")  # Размер окна
root.resizable(False, False)  # Запрет изменения размеров окна

# Поле ввода текста
frame_text = tk.Frame(root)
frame_text.pack(pady=10)
tk.Label(frame_text, text="Введите текст:").pack(anchor="w", padx=5)
entry_text = tk.Text(frame_text, height=10, width=60)
entry_text.pack(padx=5, pady=5)

# Поле ввода шаблона
frame_pattern = tk.Frame(root)
frame_pattern.pack(pady=10)
tk.Label(frame_pattern, text="Введите подстроку:").pack(anchor="w", padx=5)
entry_pattern = tk.Entry(frame_pattern, width=40)
entry_pattern.pack(padx=5, pady=5)

# Кнопка для запуска поиска
button_build = tk.Button(root, text="Построить DFA и поиск", command=display_automaton_and_search)
button_build.pack(pady=10)

# Таблица DFA
frame_table = tk.Frame(root)
frame_table.pack(pady=10)

# Метка для отображения результатов
results_label = tk.Label(root, text="Search results will be displayed here.", wraplength=480, justify="left")
results_label.pack(pady=10)

# Запуск приложения
root.mainloop()
