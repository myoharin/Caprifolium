import random as r
factorial_cache = {0:1}
def factorial(m:int) -> float:
    if m not in factorial_cache:
        factorial_cache[m] = m*factorial(m-1)
    return float(factorial_cache[m])

def n_choose_r(n:int, r:int) -> float:
    if r == 0 or r == n: return 1
    if r == 1: return n
    if r > n: return 0
    else: return factorial(n) / (factorial(r) * factorial(n - r))


def node_to_link(seed_count: int, node_dices: list[int]) -> int: # works and DONE
    accumulator = 0
    for i in range(seed_count):
        if (node_dices[i] >= i+1):
            accumulator += n_choose_r(node_dices[i], i+1)
    return accumulator

def link_to_node(seed_count: int, link_index: int) -> list[int]:
    node_dices = []
    for i in range(seed_count, 0, -1):
        j = i
        while n_choose_r(j, i) <= link_index:
            j += 1
        node_dices.append(j-1)
        link_index -= n_choose_r(j-1, i)
    node_dices.reverse()
    return node_dices

def process(seed_count: int, node_dices):
    print(f"{seed_count} | {node_dices} -> {int(node_to_link(seed_count, node_dices))} -> {link_to_node(seed_count, int(node_to_link(seed_count, node_dices)))}")

for _ in range(10):
    # generate random node index [n0, n1, n2]
    node_dices = []
    node_dices.clear()
    seed_count = r.randint(1,5)
    for _ in range(seed_count):
        r_num = r.randint(0, 4)
        while (r_num in node_dices):
            r_num = r.randint(0, 6)
        node_dices.append(r_num)
    node_dices.sort()
    
    process(seed_count, node_dices)

# n = 4
# for i in range(n,10):
#     print(int(n_choose_r(i,n)))