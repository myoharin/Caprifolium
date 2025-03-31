class Lonicera:
    def __init__(self, growth=None, grow=False, nodes=None, links=None):
        self._nodes = nodes if nodes is not None else []
        self._links = links if links is not None else []
        self.Growth = growth
        self.GrowthSynced = False

        for _ in range(self.calculate_vine_count()):
            self._links.append(None)

        if self.Growth is not None and grow:
            self.grow()

    @property
    def node_count(self):
        return len(self._nodes)

    @property
    def link_count(self):
        return len(self._links)

    @property
    def root_node(self):
        return self._nodes[0] if self.node_count > 0 else None

    @property
    def nodes(self):
        return self._nodes

    @property
    def links(self):
        return self._links

    def calculate_vine_count(self):
        return int((self.node_count * (self.node_count + 1)) // 2)

    def get_value(self, n0, n1):
        if n0 >= self.node_count or n1 >= self.node_count:
            raise IndexError("Node index out of range")
        return self._links[self.nodes_to_link_index(n0, n1)]

    def add(self, new_node, grow_new=True):
        for i in range(self.node_count):
            if grow_new and self.Growth is not None:
                self._links.append(self.Growth(self._nodes[i], new_node))
            else:
                self._links.append(None)
        self._nodes.append(new_node)

    def insert(self, index, new_node, grow_new=True):
        self._nodes.insert(index, new_node)
        row_pivot = 0 if index == 0 else self.nodes_to_link_index(0, index - 1)
        
        cache_link = None
        for i in range(index):
            if grow_new and self.Growth is not None:
                cache_link = self.Growth(self._nodes[i], self._nodes[index])
            self._links.insert(row_pivot + i, cache_link)

        for i in range(index + 1, self.node_count + 1):
            if grow_new and self.Growth is not None:
                cache_link = self.Growth(self._nodes[index], self._nodes[i])
            self._links.insert(self.nodes_to_link_index(index, i), cache_link)

    def add_range(self, new_node_range, grow_new=True):
        for node in new_node_range:
            self.add(node, grow_new)

    def insert_range(self, index, new_node_range, grow_new=True):
        for node in new_node_range:
            self.insert(index, node, grow_new)
            index += 1

    def remove(self, target):
        for i in range(self.node_count):
            if self._nodes[i] is not None and self._nodes[i] == target:
                self.remove_at(i)
                return True
        return False

    def remove_at(self, index):
        if index != 0:
            del self._links[self.nodes_to_link_index(0, index):self.nodes_to_link_index(0, index) + index]

        for i in range(self.node_count - index - 1):
            del self._links[self.nodes_to_link_index(0, i + index + 1) - i]

        del self._nodes[index]

    def remove_range(self, index, count):
        for _ in range(count):
            self.remove_at(index)

    def update_nodes(self, func):
        self.GrowthSynced = False
        for i in range(self.node_count):
            self._nodes[i] = func(self._nodes[i])

    def update_links(self, func):
        self.GrowthSynced = False
        for i in range(self.link_count):
            self._links[i] = func(self._links[i])

    def grow(self):
        if self.node_count <= 1 or self.Growth is None:
            return
        self.GrowthSynced = True
        index = 0
        for i in range(self.node_count):
            for j in range(i + 1, self.node_count):
                self._links[index] = self.Growth(self._nodes[i], self._nodes[j])
                index += 1

    def clear(self):
        self._nodes.clear()
        self._links.clear()

    @staticmethod
    def nodes_to_link_index(n0, n1):
        if n1 < n0:
            n0, n1 = n1, n0
        if n1 == n0:
            raise ValueError("The two node indices cannot be the same.")
        return (n1 * (n1 - 1)) // 2 + n0